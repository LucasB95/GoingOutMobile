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
    public partial class FavoritesViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        RestaurantResponse restaurantSelected;

        [ObservableProperty]
        ObservableCollection<RestaurantResponse> reserveCollection;

        [ObservableProperty]
        private bool isActivity = false;

        [ObservableProperty]
        bool isRefreshing;

        public Command GetDataCommand { get; set; }

        private readonly INavegacionService _navegacionService;
        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly IRestaurantService _restaurantService;

        public FavoritesViewModel(INavegacionService navegacionService, IGenericQueriesServices genericQueriesServices, IRestaurantService restaurantService)
        {
            _navegacionService = navegacionService;
            _genericQueriesServices = genericQueriesServices;
            _restaurantService = restaurantService;

            GetDataCommand = new Command(async () => await Refresh());
            GetDataCommand.Execute(this);
            PropertyChanged += ReserveListViewModel_PropertyChanged;
        }

        private async void ReserveListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RestaurantSelected))
            {
                var uri = $"{nameof(RestaurantDetailPage)}?id={RestaurantSelected.IdClient}&page={nameof(FavoritesPage)}";
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
                var UserId = Preferences.Get("IdUser", string.Empty);
                var listBooking = await _restaurantService.GetFavorites(UserId);

                if (listBooking != null && listBooking.Count > 0) { ReserveCollection = new ObservableCollection<RestaurantResponse>(listBooking); }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", "No se pudo cargar la lista de favoritos o no posee ninguno", "Aceptar");
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
        }

        [RelayCommand]
        async Task Refresh()
        {
            GetDataCommand = new Command(async () => await LoadDataAsync());
            GetDataCommand.Execute(this);

            IsRefreshing = false;
        }
    }
}
