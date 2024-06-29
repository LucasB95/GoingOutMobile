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

        private readonly IMaps _maps;
        public RestaurantFindListViewModel(IGenericQueriesServices genericQueriesServices, IRestaurantService restaurantService, INavegacionService navegacionService, IMaps maps)
        {
            _genericQueriesServices = genericQueriesServices;
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
            _maps = maps;
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

        [RelayCommand]
        async Task BuscarRestaurantesCercanos()
        {
            //string apiKey = "TU_API_KEY_DE_GOOGLE";

            //var direcciones = new List<AdressResponse>
            //    {
            //        new Direccion { Calle = "Tu Calle", Numero = "1234", Localidad = "Tu Localidad", Provincia = "Tu Provincia" },
            //        // Otras direcciones...
            //    };

            //// Geocodifica las direcciones
            //await _maps.GeocodificarDireccionesAsync(direcciones, apiKey);

            //// Obtiene la ubicacion del dispositivo
            //var ubicacionActual = await _maps.ObtenerUbicacionActualAsync();

            //// Encuentra direcciones cercanas en un radio de 10Km
            //var direccionesCercanas = _maps.ObtenerDireccionesCercanas(ubicacionActual, direcciones, 10);

            //// Muestra las direcciones cercanas
            //foreach (var direccion in direccionesCercanas)
            //{
            //    Console.WriteLine($"{direccion.Calle} {direccion.Numero}, {direccion.Localidad}, {direccion.Provincia}");
            //}
        }


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
