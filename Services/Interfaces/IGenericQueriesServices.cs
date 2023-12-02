using GoingOutMobile.Models.Login;
using GoingOutMobile.Models.Restaurant;

namespace GoingOutMobile.Services.Interfaces
{
    public interface IGenericQueriesServices
    {
        Task<UserInformation> GetInfoUser(string username);
    }
}