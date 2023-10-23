using GoingOutMobile.Models.Config;
using GoingOutMobile.Models.Login;
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

            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("bearer", Preferences.Get("tokenGoingOut", string.Empty));

        }

        public async Task<UserInformation> GetInfoUser(string username)
        {
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

    }
}
