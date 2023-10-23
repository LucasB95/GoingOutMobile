namespace GoingOutMobile.Services.Interfaces
{
    public interface INavegacionService
    {
        Task GoToAsync(string uri);
        Task GoToAsync(string uri, IDictionary<string, object> parameters);
    }
}