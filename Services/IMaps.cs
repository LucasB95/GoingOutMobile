using GoingOutMobile.Models.Restaurant;

namespace GoingOutMobile.Services
{
    public interface IMaps
    {
        Task<Location> GeocodificarDireccionAsync(string direccion);
        Task GeocodificarDireccionesAsync(List<AdressResponse> direcciones);
        List<AdressResponse> ObtenerDireccionesCercanas(Location ubicacionActual, List<AdressResponse> direcciones, double distanciaMaximaKm);
        Task<Location> ObtenerUbicacionActualAsync();
    }
}