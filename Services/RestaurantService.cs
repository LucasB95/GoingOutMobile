using Microsoft.Extensions.Configuration;
using GoingOutMobile.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GoingOutMobile.Services
{
    public class RestaurantService
    {
        private HttpClient client;
        private Settings settings;

        public RestaurantService(HttpClient client, IConfiguration configuration)
        {
            this.client = client;

            settings = configuration.GetRequiredSection(nameof(Settings)).Get<Settings>();

        }

        //public async Task<bool> Login(string username, string password)
        //{
        //    client.DefaultRequestHeaders.Clear();
        //    client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
        //    client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

        //    var url = $"{settings.UrlBase}/Authentication/Login";

        //    var loginRequest = new LoginRequest
        //    {
        //        userName = username,
        //        userPassword = password
        //    };

        //    var json = JsonConvert.SerializeObject(loginRequest);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await client.PostAsync(url, content);

        //    if (!response.IsSuccessStatusCode) return false;

        //    var jsonResult = await response.Content.ReadAsStringAsync();

        //    var resultado = JsonConvert.DeserializeObject<UserResponse>(jsonResult);

        //    if (resultado != null && resultado.message.Contains("MSG_LOGIN_OK"))
        //    {
        //        Preferences.Set("tokenGoingOut", resultado.tokenGoingOut);
        //        Preferences.Set("IdUser", resultado.id);
        //        Preferences.Set("userName", loginRequest.userName);
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //    return true;


        //}

        //public async Task<bool> Logout(string IdUser)
        //{
        //    client.DefaultRequestHeaders.Clear();
        //    client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
        //    client.DefaultRequestHeaders.Authorization = new
        //    AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

        //    var url = $"{settings.UrlBase}/Authentication/Logout";

        //    var logoutRequest = new LogoutRequest
        //    {
        //        userId = IdUser
        //    };

        //    var json = JsonConvert.SerializeObject(logoutRequest);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await client.PostAsync(url, content);

        //    if (!response.IsSuccessStatusCode) return false;

        //    var jsonResult = await response.Content.ReadAsStringAsync();

        //    var resultado = JsonConvert.DeserializeObject<LogoutResponse>(jsonResult);

        //    if (resultado != null && resultado.DESCRIPCION_DS.Contains("MSG_LOGOUT_OK"))
        //    {
        //        return true;
        //    }

        //    return false;


        //}
    }
}
