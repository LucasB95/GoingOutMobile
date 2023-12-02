using Microsoft.Extensions.Configuration;
using GoingOutMobile.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using GoingOutMobile.Models.Restaurant;
using System.IdentityModel.Tokens.Jwt;
using GoingOutMobile.Views;
using GoingOutMobile.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GoingOutMobile.Services
{
    public class RestaurantService : IRestaurantService
    {
        private HttpClient client;
        private Settings settings;

        private readonly INavegacionService _navegacionService;

        public RestaurantService(HttpClient client, IConfiguration configuration, INavegacionService navegacionService)
        {
            this.client = client;
            _navegacionService = navegacionService;

            settings = configuration.GetRequiredSection(nameof(Settings)).Get<Settings>();
        }

        public async Task ValidaToken()
        {
            var accessToken = Preferences.Get("tokenGoingOut", string.Empty);

            if (accessToken != null && !String.IsNullOrEmpty(accessToken))
            {
                var jwt_token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                var time = jwt_token.ValidTo;

                var tiempoToken = DateTime.Compare(time, DateTime.UtcNow);
                if (time < DateTime.UtcNow)
                {
                    var uri = $"{nameof(HomePage)}";
                    await _navegacionService.GoToAsync(uri);
                }

            }
        }

        public async Task<RestaurantResponse> GetClient(string idRestaurant)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Clients/GetClient";

            var json = JsonConvert.SerializeObject(idRestaurant);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) throw new ArgumentNullException(nameof(response));

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RestaurantResponse>(jsonResult);
        }

        #region Favorites
        public async Task<List<RestaurantResponse>> GetFavorites(string idUser)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Favorites/{idUser}";

            var resultado = await client.GetAsync(uri);

            if (!resultado.IsSuccessStatusCode) return new List<RestaurantResponse>();

            var jsonResult = await resultado.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<RestaurantResponse>>(jsonResult);
        }
        public async Task<bool> SaveFavorite(FavoriteRequest favoriteRequest)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Favorites/AddFavorites";

            if (favoriteRequest == null
                || String.IsNullOrEmpty(favoriteRequest.userId)
                || String.IsNullOrEmpty(favoriteRequest.clientsId))
            {
                return false;
            }

            var json = JsonConvert.SerializeObject(favoriteRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode) return false;

            return true;
        }
        public async Task<bool> DeleteFavorite(FavoriteRequest favoriteRequest)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Favorites/DelFavorites";

            if (favoriteRequest == null
                || String.IsNullOrEmpty(favoriteRequest.userId)
                || String.IsNullOrEmpty(favoriteRequest.clientsId))
            {
                return false;
            }

            var json = JsonConvert.SerializeObject(favoriteRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode) return false;

            return true;
        }
        #endregion

        public async Task<List<CategoriesMobileResponse>> GetCategories()
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Categories";

            var resultado = await client.GetAsync(uri);

            var jsonResult = await resultado.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CategoriesMobileResponse>>(jsonResult);
        }
        public async Task<IEnumerable<RestaurantResponse>> GetCategoriesClientes(string nameCategory)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Clients/GetCategories";

            if (string.IsNullOrEmpty(nameCategory))
            {
                throw new ArgumentNullException(nameof(nameCategory));
            }

            var json = JsonConvert.SerializeObject(nameCategory);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) throw new ArgumentNullException(nameof(response));

            var jsonResult = await response.Content.ReadAsStringAsync();

            return (IEnumerable<RestaurantResponse>)JsonConvert.DeserializeObject<List<RestaurantResponse>>(jsonResult);
        }
        public async Task<MenuResponse> GetClientMenu(string idRestaurant)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Clients/GetClientMenu";

            var json = JsonConvert.SerializeObject(idRestaurant);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) throw new ArgumentNullException(nameof(response));

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MenuResponse>(jsonResult);
        }
    }
}
