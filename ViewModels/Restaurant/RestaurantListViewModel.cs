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
    public partial class RestaurantListViewModel : ViewModelGlobal, IQueryAttributable
    {
        [ObservableProperty]
        RestaurantResponse restaurantSelected;

        [ObservableProperty]
        ObservableCollection<RestaurantResponse> restaurant;

        [ObservableProperty]
        private string nameCategory;

        [ObservableProperty]
        private bool isActivity = false;

        private readonly INavegacionService _navegacionService;

        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly IRestaurantService _restaurantService;

        public RestaurantListViewModel(INavegacionService navegacionService, IGenericQueriesServices genericQueriesServices, IRestaurantService restaurantService)
        {
            _navegacionService = navegacionService;
            _genericQueriesServices = genericQueriesServices;
            PropertyChanged += RestaurantListViewModel_PropertyChanged;
            _restaurantService = restaurantService;
        }

        private async void RestaurantListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RestaurantSelected))
            {
                var uri = $"{nameof(RestaurantDetailPage)}?id={RestaurantSelected.IdClient}&page={nameof(RestaurantListPage)}";
                await _navegacionService.GoToAsync(uri);
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            NameCategory = query["nameCategory"].ToString();
            Preferences.Set("nameCategory", NameCategory);
            await LoadDataAsync(nameCategory);

        }

        public async Task LoadDataAsync(string nameCategory)
        {
            IsActivity = true;

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var listRestaurant = await _restaurantService.GetCategoriesClientes(nameCategory);
                Restaurant = new ObservableCollection<RestaurantResponse>(listRestaurant);
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
        async Task GetBackEvent()
        {
            var uri = $"//{nameof(HomePage)}";
            await _navegacionService.GoToAsync(uri);
        }

    }
}
