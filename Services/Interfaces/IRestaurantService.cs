using GoingOutMobile.Models.Restaurant;

namespace GoingOutMobile.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantResponse> GetClient(string idRestaurant);
        Task<List<RestaurantResponse>> GetFavorites(string idUser);
        Task<bool> SaveFavorite(FavoriteRequest favoriteRequest);
        Task<bool> DeleteFavorite(FavoriteRequest favoriteRequest);
        Task<List<CategoriesMobileResponse>> GetCategories();
        Task<IEnumerable<RestaurantResponse>> GetCategoriesClientes(string nameCategory);
        Task<MenuResponse> GetClientMenu(string idRestaurant);
        Task<IEnumerable<RestaurantResponse>> GetRestaurantSearch(string search);
    }
}