using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoingOutMobile.Models.Config;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace GoingOutMobile.Services
{

    public class Maps : IMaps
    {
        private Settings settings;

        private readonly INavegacionService _navegacionService;

        public Maps(INavegacionService navegacionService, IConfiguration configuration)
        {
            settings = configuration.GetRequiredSection(nameof(Settings)).Get<Settings>();
            _navegacionService = navegacionService;
        }
        public async Task GeocodificarDireccionesAsync(List<AdressResponse> direcciones)
        {
            foreach (var direccion in direcciones)
            {
                string direccionCompleta = $"{direccion.Street} {direccion.Numeration}, {direccion.Location}, {direccion.Province}";
                direccion.Coordenadas = await GeocodificarDireccionAsync(direccionCompleta);
            }

        }

        public async Task<Location> GeocodificarDireccionAsync(string direccion)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(direccion)}&format=json&addressdetails=1&limit=1";

            using (var client = new HttpClient())
            {
                // Nominatim requiere que se especifique el User-Agent para identificar la aplicación que realiza la solicitud.
                client.DefaultRequestHeaders.Add("User-Agent", "GoingOutMobile (soportegoingout@gmail.com)");

                var response = await client.GetStringAsync(url);
                var json = JArray.Parse(response);

                if (json.Count > 0)
                {
                    var location = json[0];
                    double lat = double.Parse(location["lat"].ToString());
                    double lon = double.Parse(location["lon"].ToString());
                    return new Location(lat, lon);
                }
            }
            return null;
        }

        //Obtiene la ubicacion Actual del usuario con su Location, devuelve la altitud y longitud
        public async Task<Location> ObtenerUbicacionActualAsync()
        {
            try
            {
                var ubicacion = await Geolocation.GetLastKnownLocationAsync();

                if (ubicacion == null)
                {
                    ubicacion = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }

                return ubicacion;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Manejar la excepción cuando la característica no es soportada en el dispositivo
                await Application.Current.MainPage.DisplayAlert("Error", "La funcionalidad de geolocalización no es soportada en este dispositivo.", "OK");
            }
            catch (PermissionException pEx)
            {
                // Manejar la excepción cuando los permisos no son concedidos
                await Application.Current.MainPage.DisplayAlert("Error", "Permiso de geolocalización denegado. Por favor, conceda acceso a la ubicación en la configuración de la aplicación.", "OK");
            }
            catch (Exception ex)
            {
                // Manejar cualquier otra excepción no específica
                await Application.Current.MainPage.DisplayAlert("Error", "Hubo un error al intentar obtener la ubicación. Por favor, intente nuevamente.", "OK");
            }

            return null;
        }

        //Busca los restaurant cercanos al usuario
        public List<AdressResponse> ObtenerDireccionesCercanas(Location ubicacionActual, List<AdressResponse> direcciones, double distanciaMaximaKm)
        {
            var direccionesCercanas = new List<AdressResponse>();

            foreach (var direccion in direcciones)
            {
                if (direccion.Coordenadas != null)
                {
                    var distancia = Location.CalculateDistance(ubicacionActual, direccion.Coordenadas, DistanceUnits.Kilometers);
                    if (distancia <= distanciaMaximaKm)
                    {
                        direccionesCercanas.Add(direccion);
                    }
                }
            }

            return direccionesCercanas;
        }

    }
}
