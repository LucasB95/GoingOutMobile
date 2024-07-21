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
        public async Task<RestaurantResponse> DetailsRestaurant(string idRestaurant)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Clients/GetClient";

            var json = JsonConvert.SerializeObject(idRestaurant);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RestaurantResponse>(jsonResult);
        }
        public async Task<IEnumerable<RestaurantResponse>> GetRestaurantSearch(string search)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentNullException(nameof(search));
            }
            var url = $"{settings.UrlBase}/Clients/GetRestaurantSearch/{search}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return (IEnumerable<RestaurantResponse>)JsonConvert.DeserializeObject<List<RestaurantResponse>>(jsonResult);
        }
        public async Task<ClientsList> GetRestaurantAdress(string page)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            if (string.IsNullOrEmpty(page))
            {
                throw new ArgumentNullException(nameof(page));
            }
            var url = $"{settings.UrlBase}/Clients/GetAdressRestaurant/{page}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ClientsList();
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ClientsList>(jsonResult);
        }

        #region Favorites
        public async Task<List<RestaurantResponse>> GetFavorites(string idUser)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Favorites/{idUser}";

            var resultado = await client.GetAsync(uri);

            if (!resultado.IsSuccessStatusCode)
            {
                var errorMessage = await resultado.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await resultado.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<RestaurantResponse>>(jsonResult);
        }
        public async Task<bool> SaveFavorite(FavoriteRequest favoriteRequest)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

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
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

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

        #region Categories y Menu
        public async Task<List<CategoriesMobileResponse>> GetCategories()
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Categories";

            var resultado = await client.GetAsync(uri);

            if (!resultado.IsSuccessStatusCode)
            {
                var errorMessage = await resultado.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await resultado.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CategoriesMobileResponse>>(jsonResult);
        }
        public async Task<IEnumerable<RestaurantResponse>> GetCategoriesClientes(string nameCategory)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Clients/GetCategories";

            if (string.IsNullOrEmpty(nameCategory))
            {
                throw new ArgumentNullException(nameof(nameCategory));
            }

            var json = JsonConvert.SerializeObject(nameCategory);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<RestaurantResponse>>(jsonResult);
        }
        public async Task<MenuResponse> GetClientMenu(string idRestaurant)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Clients/GetClientMenu";

            var json = JsonConvert.SerializeObject(idRestaurant);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MenuResponse>(jsonResult);
        }

        #endregion

        #region Reservas
        public async Task<bool> NewReserve(BookingCreate bookingCreate)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Bookings/NewReserve";

            var json = JsonConvert.SerializeObject(bookingCreate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            return true;
        }
        
        public async Task<bool> EditReservation(BookingResponse bookingCreate)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Bookings/EditReservation";

            var json = JsonConvert.SerializeObject(bookingCreate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            return true;
        }
        public async Task<IEnumerable<Booking>> GetBookings(string IdUser)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            if (string.IsNullOrEmpty(IdUser))
            {
                throw new ArgumentNullException(nameof(IdUser));
            }

            var uri = $"{settings.UrlBase}/Bookings/ListBooking/{IdUser}";

            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Booking>();
            }


            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Booking>>(jsonResult);
        }
        public async Task<Booking> GetBookingsRestaurant(string IdUser, string IdBooking)
        {
            await ValidaToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            if (string.IsNullOrEmpty(IdUser) || string.IsNullOrEmpty(IdBooking))
            {
                throw new ArgumentNullException(nameof(IdUser));
            }

            var uri = $"{settings.UrlBase}/Bookings/{IdUser}&{IdBooking}";

            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Booking>(jsonResult);
        }

        #endregion
    }
}
