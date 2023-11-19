using GoingOutMobile.Models.Config;
using GoingOutMobile.Models.Login;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Services
{
    public class GenericQueriesServices : IGenericQueriesServices
    {
        private HttpClient client;
        private Settings settings;

        public GenericQueriesServices(HttpClient client, IConfiguration configuration)
        {
            this.client = client;

            settings = configuration.GetRequiredSection(nameof(Settings)).Get<Settings>();

        }

        public async Task<UserInformation> GetInfoUser(string username)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            UserInformation userInformation = new UserInformation();

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var url = $"{settings.UrlBase}/Authentication/GetInfoUser";

            var json = JsonConvert.SerializeObject(username);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) throw new ArgumentNullException(nameof(response)); ;

            var jsonResult = await response.Content.ReadAsStringAsync();

            userInformation = JsonConvert.DeserializeObject<UserInformation>(jsonResult);

            return userInformation;

        }

        public async Task<List<CategoriesMobileResponse>> GetCategories()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var uri = $"{settings.UrlBase}/Categories";

            var resultado = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<List<CategoriesMobileResponse>>(resultado);
        }

        public async Task<IEnumerable<RestaurantResponse>> GetCategoriesClientes(string nameCategory)
        {
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


    }
}
