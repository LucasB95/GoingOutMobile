using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoingOutMobile.ViewModels
{
    public partial class ReserveDetailViewModel : ViewModelGlobal, IQueryAttributable, INotifyPropertyChanged
    {
        [ObservableProperty]
        private string imagenSource;

        [ObservableProperty]
        Booking reserve;

        [ObservableProperty]
        private RestaurantResponse restaurant;

        [ObservableProperty]
        private bool isActivity = false;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        private string idBooking;

        [ObservableProperty]
        private string idClient;

        [ObservableProperty]
        public string cantidadSelected;

        [ObservableProperty]
        public DateTime selectedDate;

        [ObservableProperty]
        public TimeSpan selectedTime;

        [ObservableProperty]
        public DateTime minDate = DateTime.Now;

        [ObservableProperty]
        public string descriptionStateUser;

        [ObservableProperty]
        private bool isActive;

        [ObservableProperty]
        private bool isVisible;

        [ObservableProperty]
        private bool isModify = false;

        [ObservableProperty]
        private MenuResponse menu;

        [ObservableProperty]
        List<CategoriesRestaurantResponse> categories;

        [ObservableProperty]
        ObservableCollection<DrinksResponse> drinks;

        [ObservableProperty]
        ObservableCollection<DishesResponse> dishes;

        public ObservableCollection<MenuCategoriesDishes> DishesList { get; private set; } = new ObservableCollection<MenuCategoriesDishes>();

        public ObservableCollection<MenuCategoriesDrinks> DrinksList { get; private set; } = new ObservableCollection<MenuCategoriesDrinks>();

        public Command GetDataCommand { get; set; }

        private readonly INavegacionService _navegacionService;
        private readonly IRestaurantService _restaurantService;
        private readonly IMercadoPagoService _mercadoPagoService;

        public ReserveDetailViewModel(IRestaurantService restaurantService, INavegacionService navegacionService, IMercadoPagoService mercadoPagoService)
        {
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
            _mercadoPagoService = mercadoPagoService;
            PropertyChanged += OnPropertyChanged;
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IdClient = query["id"].ToString();
            IdBooking = query["idBooking"].ToString();

            IsRefreshing = true;
            await RefreshCommand.ExecuteAsync(this);
        }
        public async Task LoadDataAsync()
        {
            IsActivity = true;

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var UserId = Preferences.Get("IdUser", string.Empty);
                Reserve = await _restaurantService.GetBookingsRestaurant(UserId, IdBooking);
                Restaurant = await _restaurantService.DetailsRestaurant(IdClient);

                if (Reserve == null || Restaurant == null)
                {
                    await Shell.Current.DisplayAlert("Mensaje", "No se pudo cargar la reserva", "Aceptar");
                }
                else
                {
                    SelectedDate = Reserve.Date;
                    SelectedTime = Reserve.Date.TimeOfDay;
                    CantidadSelected = Reserve.AmountPeople.ToString();
                    IsActive = Reserve.Active;
                    DescriptionStateUser = Reserve.DescriptionStateUser;

                    var favorites = await _restaurantService.GetFavorites(Preferences.Get("UserId", string.Empty));

                    if (favorites != null || favorites.Count() > 0)
                    {
                        Restaurant.IsBookmarkEnabled = favorites.Any(x => x.IdClient == Restaurant.IdClient);
                    }

                    ImagenSource = Restaurant.IsBookmarkEnabled ? "bookmark_fill_icon" : "bookmark_empty_icon";

                    #region Menu
                    Menu = await _restaurantService.GetClientMenu(IdClient);

                    this.Categories = new List<CategoriesRestaurantResponse>(Menu.Categories);
                    Dishes = new ObservableCollection<DishesResponse>(Menu.Dishes);
                    Drinks = new ObservableCollection<DrinksResponse>(Menu.Drinks);

                    var CategoriesId = this.Categories.Select(c => c.Id).ToList();

                    var DrinksCategoriesId = Drinks.Where(x => CategoriesId.Contains(x.Categories.Id)).Select(x => x.Categories.Id).ToList();

                    var DishesCategoriesId = Dishes.Where(x => CategoriesId.Contains(x.Categories.Id)).Select(x => x.Categories.Id).ToList();

                    MenuCategoriesDishes postres = null;

                    foreach (var item in this.Categories)
                    {
                        if (DishesCategoriesId.Any(x => x == item.Id))
                        {
                            if (item.Name.Contains("Postres"))
                            {
                                postres = new MenuCategoriesDishes(item.Id, item.Name, Dishes.Where(x => x.Categories.Id == item.Id).ToList());
                            }
                            else
                            {
                                DishesList.Add(new MenuCategoriesDishes(item.Id, item.Name, Dishes.Where(x => x.Categories.Id == item.Id).ToList()));
                            }
                        }
                        if (DrinksCategoriesId.Any(x => x == item.Id))
                        {
                            DrinksList.Add(new MenuCategoriesDrinks(item.Id, item.Name, Drinks.Where(x => x.Categories.Id == item.Id).ToList()));
                        }
                    }

                    if (postres != null) { DishesList.Add(postres); }

                    DishesList.OrderBy(x => x.IdCategory).ToList();
                    DrinksList.OrderBy(x => x.IdCategory).ToList();

                    #endregion
                }



            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar");
            }
            finally
            {
                IsBusy = false;
            }

            IsActivity = false;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsActivity)
                return;


            if (e.PropertyName == nameof(CantidadSelected)
                || e.PropertyName == nameof(SelectedDate)
                || e.PropertyName == nameof(SelectedTime)
                || e.PropertyName == nameof(DescriptionStateUser)
                || e.PropertyName == nameof(IsActive))
            {
                IsModify = !Reserve.StateClient && !String.IsNullOrEmpty(Reserve.DescriptionStateClient) ? false : true;
            }
        }

        [RelayCommand]
        async Task ReserveModify()
        {
            IsActivity = true;
            IsModify = false;

            if (String.IsNullOrEmpty(CantidadSelected))
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccione la cantidad de personas", "Aceptar");
            }
            else if (SelectedDate.Date == DateTime.MinValue)
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccioné el dia para la reserva", "Aceptar");
            }
            else if (SelectedTime == TimeSpan.MinValue)
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccioné el dia para la reserva", "Aceptar");
            }
            else if (!String.IsNullOrEmpty(CantidadSelected) && !String.IsNullOrEmpty(SelectedDate.ToString()))
            {

                DateTime fechaConHora = SelectedDate.Date + SelectedTime;

                if (fechaConHora < DateTime.Now)
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Seleccioné correctamente la Hora para la reserva", "Aceptar");
                }
                else
                {

                    BookingResponse bookingResponse = new BookingResponse()
                    {
                        Id = Reserve.Id,
                        AmountPeople = int.Parse(CantidadSelected),
                        Date = fechaConHora,
                        Active = IsActive,
                        UserId = Reserve.UserId,
                        ClientsId = Reserve.ClientsId,
                        BusinessName = Reserve.BusinessName,
                        StateClient = Reserve.StateClient,
                        DescriptionStateClient = Reserve.DescriptionStateClient,
                        DescriptionStateUser = !String.IsNullOrEmpty(DescriptionStateUser) ? DescriptionStateUser : "",
                        BookingComplete = false
                    };

                    var UserId = Preferences.Get("IdUser", string.Empty);
                    IEnumerable<Booking> bookings = await _restaurantService.GetBookings(UserId);

                    bool ValidoReserva = false;
                    if (bookings != null && bookings.Any())
                    {
                        ValidoReserva = bookings.Any(x => x.ClientsId == bookingResponse.ClientsId && x.UserId == bookingResponse.UserId && x.Date == bookingResponse.Date
                                                    && x.AmountPeople == bookingResponse.AmountPeople);
                    }

                    if (ValidoReserva)
                    {
                        await Shell.Current.DisplayAlert("Mensaje", "Ya existe una reserva para ese dia y con esa hora", "Aceptar");
                    }
                    else
                    {
                        try
                        {

                            if (await _restaurantService.EditReservation(bookingResponse))
                            {
                                //Llevar a la lista de reservas desde el menu costadp
                                var uri = $"//{nameof(ReserveListPage)}?page=ReserveDetail";
                                await _navegacionService.GoToAsync(uri);

                            }
                            else
                            {
                                await Shell.Current.DisplayAlert("Mensaje", "No se pudo generar la reserva", "Aceptar");
                            }


                        }
                        catch (Exception e)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar");
                        }
                    }

                }

                //var resultado = await _mercadoPagoService.preparandoMP();
                //var uri = new Uri(resultado[1]);
                //await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

                //await Shell.Current.DisplayAlert("Mensaje", "sacar la cantidad desde el picker", "Aceptar");
            }

            IsActivity = false;
        }

        [RelayCommand]
        public async void ChangeIsVisible()
        {
            if (IsVisible)
            {
                IsVisible = false;
            }
            else
            {
                IsVisible = true;
            }
        }

        [RelayCommand]
        async Task Refresh()
        {
            GetDataCommand = new Command(async () => await LoadDataAsync());
            GetDataCommand.Execute(this);

            IsRefreshing = false;
        }

        [RelayCommand]
        async Task MapsEvent()
        {

            string address = Restaurant.Adress.Numeration + " " + Restaurant.Adress.Street + ", " + Restaurant.Adress.Location + ", " + Restaurant.Adress.Province;
            string encodedAddress = Uri.EscapeDataString(address);

            // Create the URI for Google Maps
            Uri mapsUri = new Uri($"https://www.google.com/maps/search/?api=1&query={encodedAddress}");

            // Open the URI
            await Launcher.Default.OpenAsync(mapsUri);
        }

        [RelayCommand]
        async Task GetBackEvent()
        {
            var uri = $"//{nameof(ReserveListPage)}?page=ReserveDetail";
            await _navegacionService.GoToAsync(uri);
        }

        [RelayCommand]
        async Task SaveFavorite()
        {
            IsActivity = true;

            if (Restaurant.IsBookmarkEnabled)
            {
                await DeleteFavorite();
            }
            else
            {
                var favorite = new FavoriteRequest
                {
                    clientsId = Restaurant.IdClient,
                    userId = Preferences.Get("UserId", string.Empty)
                };

                if (await _restaurantService.SaveFavorite(favorite))
                {
                    IdClient = Restaurant.IdClient;
                    await RefreshCommand.ExecuteAsync(this);
                    //await LoadDataAsync(restaurant.IdClient);

                }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", "No se pudo guardar el nuevo favorito", "Aceptar");
                }
            }

            IsActivity = false;
        }

        [RelayCommand]
        async Task DeleteFavorite()
        {
            IsActivity = true;

            var favorite = new FavoriteRequest
            {
                clientsId = Restaurant.IdClient,
                userId = Preferences.Get("UserId", string.Empty)
            };

            if (await _restaurantService.DeleteFavorite(favorite))
            {
                IdClient = Restaurant.IdClient;
                await RefreshCommand.ExecuteAsync(this);
                //await LoadDataAsync(restaurant.IdClient);
            }
            else
            {
                await Shell.Current.DisplayAlert("Mensaje", "No se pudo eliminar el favorito", "Aceptar");
            }

            IsActivity = false;
        }


        [RelayCommand]
        async Task ProbarMP()
        {
            bool usuarioAcepto = await Shell.Current.DisplayAlert("Información", "Abonando por Mercado Pago, el restaurant puede guardar la mesa hasta 45 minutos de la hora pactada dentro de la reserva. ¿Quiere abonarlo?", "Sí", "No");

            if (usuarioAcepto)
            {
                //var resultado1 = await _mercadoPagoService.MPCorto();
                var resultado = await _mercadoPagoService.preparandoMP();

                Uri uri = new Uri(resultado[1]);
                //await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

                // Open the URI
                await Launcher.Default.OpenAsync(uri);
                //var uri = $"{nameof(MercadoPagoPage)}";
                //await _navegacionService.GoToAsync(uri);
            }


        }

    }
}
