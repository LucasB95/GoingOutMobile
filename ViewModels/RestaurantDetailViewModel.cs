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

namespace GoingOutMobile.ViewModels
{
    public partial class RestaurantDetailViewModel : ViewModelGlobal, IQueryAttributable
    {

        [ObservableProperty]
        private string imagenSource;

        [ObservableProperty]
        private RestaurantResponse restaurant;

        [ObservableProperty]
        private MenuResponse menu;

        [ObservableProperty]
        List<CategoriesRestaurantResponse> categories;

        [ObservableProperty]
        ObservableCollection<DrinksResponse> drinks;

        [ObservableProperty]
        ObservableCollection<DishesResponse> dishes;

        [ObservableProperty]
        DrinksResponse drinkSelected;

        [ObservableProperty]
        DishesResponse dishSelected;

        public ObservableCollection<MenuCategoriesDishes> DishesList { get; private set; } = new ObservableCollection<MenuCategoriesDishes>();

        public ObservableCollection<MenuCategoriesDrinks> DrinksList { get; private set; } = new ObservableCollection<MenuCategoriesDrinks>();

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        private string idClient;

        [ObservableProperty]
        private string pageReturn;

        [ObservableProperty]
        private bool isActivity = false;

        public Command GetDataCommand { get; set; }

        private readonly IRestaurantService _restaurantService;
        private readonly INavegacionService _navegacionService;
        private readonly IMercadoPagoService _mercadoPagoService;

        public RestaurantDetailViewModel(IRestaurantService restaurantService, INavegacionService navegacionService, IMercadoPagoService mercadoPagoService)
        {
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task LoadDataAsync()
        {
            IsActivity = true;

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Restaurant = await _restaurantService.GetClient(IdClient);
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

                var favorites = await _restaurantService.GetFavorites(Preferences.Get("UserId", string.Empty));

                if (favorites != null || favorites.Count() > 0)
                {
                    Restaurant.IsBookmarkEnabled = favorites.Any(x => x.IdClient == Restaurant.IdClient);
                }

                ImagenSource = Restaurant.IsBookmarkEnabled ? "bookmark_fill_icon" : "bookmark_empty_icon";
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


        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IdClient = query["id"].ToString();
            PageReturn = query["page"].ToString();

            IsRefreshing = true;
            await RefreshCommand.ExecuteAsync(this);
        }

        [RelayCommand]
        async Task Refresh()
        {
            GetDataCommand = new Command(async () => await LoadDataAsync());
            GetDataCommand.Execute(this);

            IsRefreshing = false;
        }

        [RelayCommand]
        async Task GetBackEvent()
        {
            var uri = $"//{nameof(HomePage)}";
            if (PageReturn.Contains("RestaurantListPage"))
            {
                var category = Preferences.Get("nameCategory", string.Empty);
                uri = $"{nameof(RestaurantListPage)}?nameCategory={category}";
            }
            else if (PageReturn.Contains("RestaurantFindListPage"))
            {
                uri = $"/{nameof(RestaurantFindListPage)}";
            }

            await _navegacionService.GoToAsync(uri);
        }

        [RelayCommand]
        async Task ProbarMP()
        {
            //var resultado1 = await _mercadoPagoService.MPCorto();
            var resultado = await _mercadoPagoService.preparandoMP();

            var uri = new Uri(resultado[1]);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);


            //var uri = $"{nameof(MercadoPagoPage)}";
            //await _navegacionService.GoToAsync(uri);
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


        [RelayCommand]
        async Task DrinksEventSelected()
        {
            MenuResponse menu = new MenuResponse();

            DrinksResponse drinks = new DrinksResponse();

            drinks = DrinkSelected;

        }

        [RelayCommand]
        async Task DishEventSelected()
        {
            MenuResponse menu = new MenuResponse();

            DrinksResponse drinks = new DrinksResponse();

            drinks = DrinkSelected;

        }

        [RelayCommand]
        async Task ReservarMesa()
        {
            var uri = $"{nameof(BookingsPage)}?id={IdClient}&page={nameof(RestaurantDetailPage)}";
            await _navegacionService.GoToAsync(uri);
        }


        //[RelayCommand]
        //async Task CallOwner()
        //{
        //    var confirmarLlamada = Application.Current.MainPage.DisplayAlert(
        //            "Marca este numero telefonico",
        //            $"Desea llamar a este numero: {Inmueble.Telefono}",
        //            "Si",
        //            "No"
        //        );

        //    if (await confirmarLlamada)
        //    {
        //        try
        //        {
        //            PhoneDialer.Open(Inmueble.Telefono);
        //        }
        //        catch (ArgumentNullException)
        //        {
        //            await Application.Current.MainPage
        //                .DisplayAlert(
        //                "No se puede realizar esta llamada",
        //                "El numero telefonico no es valido",
        //                "Ok");
        //        }
        //        catch (FeatureNotSupportedException)
        //        {
        //            await Application.Current.MainPage
        //                .DisplayAlert(
        //                "No se puede realizar esta llamada",
        //                "El dispositivo no soporta llamadas telefonicas",
        //                "Ok");
        //        }
        //        catch (Exception)
        //        {
        //            await Application.Current.MainPage
        //                .DisplayAlert(
        //                "No se puede realizar esta llamada",
        //                "Errores en la marcacion del numero",
        //                "Ok");
        //        }

        //    }

        //}



        //[RelayCommand]
        //async Task TextMessageOwner()
        //{

        //    var message = new SmsMessage("Hola, por favor enviame info sobre la vivienda", Inmueble.Telefono);
        //    var confirmarMensajeTexto = Application.Current.MainPage.DisplayAlert(
        //            "Envia un mensaje de texto",
        //            $"Desea enviar un mensaje de texto a este numero: {Inmueble.Telefono}",
        //            "Si",
        //            "No"
        //        );

        //    if (await confirmarMensajeTexto)
        //    {
        //        try
        //        {
        //            await Sms.ComposeAsync(message);
        //        }
        //        catch (ArgumentNullException)
        //        {
        //            await Application.Current.MainPage
        //                .DisplayAlert(
        //                "No se puede enviar este sms",
        //                "El numero telefonico no es valido",
        //                "Ok");
        //        }
        //        catch (FeatureNotSupportedException)
        //        {
        //            await Application.Current.MainPage
        //                .DisplayAlert(
        //               "No se puede enviar este sms",
        //                "El dispositivo no soporta envio de sms",
        //                "Ok");
        //        }
        //        catch (Exception)
        //        {
        //            await Application.Current.MainPage
        //                .DisplayAlert(
        //               "No se puede enviar este sms",
        //                "Errores en el envio del sms",
        //                "Ok");
        //        }

        //    }

        //}


    }



}
