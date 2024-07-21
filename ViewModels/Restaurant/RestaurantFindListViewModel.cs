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

        [ObservableProperty]
        private bool isRestaurantListEmpty = false;

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
            IsRestaurantListEmpty = false;

            var restaurantList = await _restaurantService.GetRestaurantSearch(SearchText);
            Restaurant = new ObservableCollection<RestaurantResponse>(restaurantList);
            PropertyChanged += RestaurantBusquedaViewModel_PropertyChanged;

            Preferences.Set("searchRestaurant", SearchText);
            if (Restaurant.Count == 0) { IsRestaurantListEmpty = true; }

            IsActivity = false;

        });

        [RelayCommand]
        async Task BuscarRestaurantesCercanos()
        {
            IsActivity = true;

            // Obtiene la ubicacion del dispositivo
            var ubicacionActual = await _maps.ObtenerUbicacionActualAsync();
            List<RestaurantResponse> resultadoRestorant = new List<RestaurantResponse>();
            int page = 2;
            IsRestaurantListEmpty = false;

            var direcciones = await _restaurantService.GetRestaurantAdress(page.ToString());

            while (direcciones != null && direcciones.Total > 0 && direcciones.ClientsResponse.Count() > 0)
            {
                List<AdressResponse> adresses = new List<AdressResponse>();

                foreach (var item in direcciones.ClientsResponse)
                {
                    adresses.Add(item.Adress);
                }

                // Geocodifica las direcciones e inserta las coordenadas en cada Direccion para luego hacer la busqueda por cercania
                await _maps.GeocodificarDireccionesAsync(adresses);

                // Encuentra direcciones cercanas en un radio de 10Km
                var direccionesCercanas = _maps.ObtenerDireccionesCercanas(ubicacionActual, adresses, 10);

                var resultadoResto = direcciones.ClientsResponse.Where(x => direccionesCercanas.Contains(x.Adress)).ToList();
                resultadoRestorant.AddRange(resultadoResto);

                if (direcciones.NextPage == true && Restaurant.Count() < 10)
                {
                    page++;
                    direcciones = await _restaurantService.GetRestaurantAdress(page.ToString());
                }
                else { direcciones = null; }

            }

            Restaurant = new ObservableCollection<RestaurantResponse>(resultadoRestorant);
            if (Restaurant.Count == 0) { IsRestaurantListEmpty = true; }

            IsActivity = false;
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
