using GoingOutMobile.Models.Config;
using GoingOutMobile.Models.Login;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace GoingOutMobile.Services
{
    public class SecurityService
    {
        private HttpClient client;
        private Settings settings;

        public SecurityService(HttpClient client,IConfiguration configuration)
        {
            this.client = client;

            settings = configuration.GetRequiredSection(nameof(Settings)).Get<Settings>();

            client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

        }

        public async Task<bool> Login(string username, string password)
        {
            var url = $"{settings.UrlBase}/Authentication/Login";

            //var url2 = "http://192.168.0.210/api/Status/isAlive";
            //var response = await client.GetAsync(url2);

            var loginRequest = new LoginRequest
            {
                userName = username,
                userPassword = password
            };

            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) return false;

            var jsonResult = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<UserResponse>(jsonResult);

            if (resultado != null && resultado.message.Contains("MSG_LOGIN_OK"))
            {
                Preferences.Set("tokenGoingOut", resultado.tokenGoingOut);
                Preferences.Set("IdUser", resultado.id);
                Preferences.Set("userName", loginRequest.userName);
            }
            else
            {
                return false;
            }

            return true;
                      

        }

        

    }
}
