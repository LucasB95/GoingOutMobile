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
    public partial class HomeViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        string nombreUsuario;

        [ObservableProperty]
        string direccion;

        [ObservableProperty]
        CategoriesMobileResponse categoriesMobileSelected;

        [ObservableProperty]
        ObservableCollection<CategoriesMobileResponse> categoriesMobiles;

        [ObservableProperty]
        ObservableCollection<RestaurantResponse> favorites;

        [ObservableProperty]
        ObservableCollection<Booking> reserveCollection;

        [ObservableProperty]
        bool isRefreshing;  
        
        [ObservableProperty]
        bool mostrarMGS;

        [ObservableProperty]
        RestaurantResponse restaurantSelected;

        public Command GetDataCommand { get; set; }

        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly IRestaurantService _restaurantService;
        private readonly INavegacionService _navegacionService;
        public HomeViewModel(IGenericQueriesServices genericQueriesServices, INavegacionService navegacionService, IRestaurantService restaurantService, IMaps maps)
        {
            _genericQueriesServices = genericQueriesServices;
            _navegacionService = navegacionService;
            _restaurantService = restaurantService;

            PropertyChanged += RestaurantDetailViewModel_PropertyChanged;
        }

        private async void RestaurantDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RestaurantSelected))
            {
                var uri = $"{nameof(RestaurantDetailPage)}?id={RestaurantSelected.IdClient}&page={nameof(HomePage)}";
                await _navegacionService.GoToAsync(uri);
            }
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var listCategories = await _restaurantService.GetCategories();
                CategoriesMobiles = new ObservableCollection<CategoriesMobileResponse>(listCategories);
                var listFavorites = await _restaurantService.GetFavorites(Preferences.Get("UserId", string.Empty));
                Favorites = new ObservableCollection<RestaurantResponse>(listFavorites);

            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task Refresh()
        {
            NombreUsuario = Preferences.Get("userName", string.Empty);
            GetDataCommand = new Command(async () => await LoadDataAsync());
            GetDataCommand.Execute(this);

            IsRefreshing = false;
        }

        [RelayCommand]
        async Task CategoryEventSelected()
        {
            var uri = $"{nameof(RestaurantListPage)}?nameCategory={CategoriesMobileSelected.Name}";
            await _navegacionService.GoToAsync(uri);
        }  

        [RelayCommand]
        async Task TapBusquedaRestaurant()
        {
            var uri = $"{nameof(RestaurantFindListPage)}";
            await _navegacionService.GoToAsync(uri);
        }
    }
}
