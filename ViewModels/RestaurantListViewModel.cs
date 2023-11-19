using CommunityToolkit.Mvvm.ComponentModel;
using GoingOutMobile.Models.Restaurant;
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

        private readonly INavegacionService _navegacionService;

        private readonly IGenericQueriesServices _genericQueriesServices;

        public RestaurantListViewModel(INavegacionService navegacionService, IGenericQueriesServices genericQueriesServices)
        {
            _navegacionService = navegacionService;
            _genericQueriesServices = genericQueriesServices;
            PropertyChanged += RestaurantListViewModel_PropertyChanged;
        }

        private async void RestaurantListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(restaurantSelected))
            {
                var uri = $"{nameof(RestaurantDetailPage)}?id={restaurantSelected.BusinessName}";
                await _navegacionService.GoToAsync(uri);
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var nameCategory = query["nameCategory"].ToString();
            await LoadDataAsync(nameCategory);

        }

        public async Task LoadDataAsync(string nameCategory)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var listRestaurant = await _genericQueriesServices.GetCategoriesClientes(nameCategory);
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

        }

    }
}
