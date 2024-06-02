using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels.Reserve
{
    public partial class LastVisitedDetailViewModel : ViewModelGlobal, IQueryAttributable
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
        public string observation;

        [ObservableProperty]
        private bool isVisible;

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

        public LastVisitedDetailViewModel(IRestaurantService restaurantService, INavegacionService navegacionService)
        {
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;

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

                if (Reserve == null || Restaurant == null)
                {
                    await Shell.Current.DisplayAlert("Mensaje", "No se pudo cargar la reserva", "Aceptar");
                }
                else
                {
                    SelectedTime = Reserve.Date.TimeOfDay;
                    SelectedDate = Reserve.Date;

                    var favorites = await _restaurantService.GetFavorites(Preferences.Get("UserId", string.Empty));

                    if (favorites != null || favorites.Count() > 0)
                    {
                        Restaurant.IsBookmarkEnabled = favorites.Any(x => x.IdClient == Restaurant.IdClient);
                    }

                    ImagenSource = Restaurant.IsBookmarkEnabled ? "bookmark_fill_icon" : "bookmark_empty_icon";

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
            var uri = $"//{nameof(LastVisitedPage)}";
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
    }
}
