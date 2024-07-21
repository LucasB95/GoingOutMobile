using GoingOutMobile.Models.Restaurant;

namespace GoingOutMobile.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantResponse> DetailsRestaurant(string idRestaurant);
        Task<List<RestaurantResponse>> GetFavorites(string idUser);
        Task<bool> SaveFavorite(FavoriteRequest favoriteRequest);
        Task<bool> DeleteFavorite(FavoriteRequest favoriteRequest);
        Task<List<CategoriesMobileResponse>> GetCategories();
        Task<IEnumerable<RestaurantResponse>> GetCategoriesClientes(string nameCategory);
        Task<MenuResponse> GetClientMenu(string idRestaurant);
        Task<IEnumerable<RestaurantResponse>> GetRestaurantSearch(string search);
        Task<bool> NewReserve(BookingCreate bookingCreate);
        Task<IEnumerable<Booking>> GetBookings(string IdUser);
        Task<Booking> GetBookingsRestaurant(string IdUser, string IdClient);
        Task<bool> EditReservation(BookingResponse bookingCreate);
        Task<ClientsList> GetRestaurantAdress(string page);
    }
}