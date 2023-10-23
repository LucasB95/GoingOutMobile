using GoingOutMobile.Models.Login;

namespace GoingOutMobile.Services.Interfaces
{
    public interface IGenericQueriesServices
    {
        Task<UserInformation> GetInfoUser(string username);
    }
}