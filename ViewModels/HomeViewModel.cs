using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class HomeViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        string nombreUsuario;

        [ObservableProperty]
        CategoriesMobileResponse categoriesMobileSelected;

        [ObservableProperty]
        ObservableCollection<CategoriesMobileResponse> categoriesMobiles;

        [ObservableProperty]
        bool isRefreshing;

        //[ObservableProperty]
        //ObservableCollection<InmuebleResponse> favoriteInmuebles;

        public Command GetDataCommand { get; set; }

        private readonly IGenericQueriesServices _genericQueriesServices;

        private readonly INavegacionService _navegacionService;

        public HomeViewModel(IGenericQueriesServices genericQueriesServices, INavegacionService navegacionService)
        {
            _genericQueriesServices = genericQueriesServices;
            _navegacionService = navegacionService;
            

        }

        public async Task LoadInfo()
        {
            NombreUsuario = Preferences.Get("userName", string.Empty);
            GetDataCommand = new Command(async () => await LoadDataAsync());
            GetDataCommand.Execute(this);

            await Task.Delay(100);
        }


        public async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var listCategories = await _genericQueriesServices.GetCategories();
                //var listClientes = await _genericQueriesServices.GetInmueblesFavoritos();

                //FavoriteInmuebles = new ObservableCollection<InmuebleResponse>(listInmuebles);
                CategoriesMobiles = new ObservableCollection<CategoriesMobileResponse>(listCategories);
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
            var uri = $"{nameof(RestaurantListPage)}?nameCategory={categoriesMobileSelected.Name}";
            await _navegacionService.GoToAsync(uri);
        }


        [RelayCommand]
        async Task TapBusquedaRestaurant()
        {
            //var uri = $"{nameof(InmuebleBusquedaPage)}";
            //await _navegacionService.GoToAsync(uri);
        }
    }
}
