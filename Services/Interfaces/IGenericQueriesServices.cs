using GoingOutMobile.Models.Login;
using GoingOutMobile.Models.Restaurant;

namespace GoingOutMobile.Services.Interfaces
{
    public interface IGenericQueriesServices
    {
        Task<List<CategoriesMobileResponse>> GetCategories();
        Task<UserInformation> GetInfoUser(string username);
        Task<IEnumerable<RestaurantResponse>> GetCategoriesClientes(string nameCategory);
    }
}