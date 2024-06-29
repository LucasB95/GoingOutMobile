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

        public async Task<Location> GeocodificarDireccionAsync(string direccion)
        {
            string apiKey = settings.GoogleMaps;

            using (var client = new HttpClient())
            {
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={direccion}&key={apiKey}";
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["status"].ToString() == "OK")
                {
                    var location = json["results"].First()["geometry"]["location"];
                    double lat = (double)location["lat"];
                    double lng = (double)location["lng"];
                    return new Location(lat, lng);
                }
            }
            return null;
        }

        public async Task GeocodificarDireccionesAsync(List<AdressResponse> direcciones, string apiKey)
        {
            foreach (var direccion in direcciones)
            {
                string direccionCompleta = $"{direccion.Street} {direccion.Numeration}, {direccion.Location}, {direccion.Province}";
                direccion.Coordenadas = await GeocodificarDireccionAsync(direccionCompleta);
            }
        }

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

        public async Task ObtenerUbicacionYDireccionAsync(string apiKey)
        {
            var ubicacion = await ObtenerUbicacionActualAsync();
            if (ubicacion != null)
            {
                var direccion = await ObtenerDireccionAsync(ubicacion.Latitude, ubicacion.Longitude, apiKey);
                await Application.Current.MainPage.DisplayAlert("Ubicación", $"Lat: {ubicacion.Latitude}, Lon: {ubicacion.Longitude}\nDirección: {direccion}", "OK");
            }
        }

        public async Task<string> ObtenerDireccionAsync(double latitud, double longitud,string _apiKey)
        {
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitud},{longitud}&key={_apiKey}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["status"].ToString() == "OK")
                {
                    var direccion = json["results"].First()["formatted_address"].ToString();
                    return direccion;
                }
                else
                {
                    return "No se pudo encontrar la dirección.";
                }
            }
        }
    }
}
