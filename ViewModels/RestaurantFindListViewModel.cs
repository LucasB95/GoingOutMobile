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
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace GoingOutMobile.ViewModels
{
    public partial class RestaurantFindListViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        RestaurantResponse restaurantSelected;

        [ObservableProperty]
        ObservableCollection<RestaurantResponse> restaurant;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private bool isActivity = false;

        private readonly IGenericQueriesServices _genericQueriesServices;

        private readonly IRestaurantService _restaurantService;

        private readonly INavegacionService _navegacionService;

        public RestaurantFindListViewModel(IGenericQueriesServices genericQueriesServices, IRestaurantService restaurantService, INavegacionService navegacionService)
        {
            _genericQueriesServices = genericQueriesServices;
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
        }

        [RelayCommand]
        async Task GetBackEvent()
        {
            var uri = $"//{nameof(HomePage)}";
            await _navegacionService.GoToAsync(uri);
        }

        public ICommand EjecutarBusqueda => new Command(async () =>
        {
            IsActivity = true;

            var restaurantList = await _restaurantService.GetRestaurantSearch(SearchText);
            Restaurant = new ObservableCollection<RestaurantResponse>(restaurantList);
            PropertyChanged += RestaurantBusquedaViewModel_PropertyChanged;

            Preferences.Set("searchRestaurant", SearchText);

            IsActivity = false;

        });

        private async void RestaurantBusquedaViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RestaurantSelected))
            {
                var uri = $"{nameof(RestaurantDetailPage)}?id={RestaurantSelected.IdClient}&page={nameof(RestaurantFindListPage)}";
                await _navegacionService.GoToAsync(uri);
            }
        }

    }
}
