using GoingOutMobile.Models.Config;
using GoingOutMobile.Models.Login;
using GoingOutMobile.Views;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GoingOutMobile.Services
{
    public class SecurityService
    {
        private HttpClient client;
        private Settings settings;

        public SecurityService(HttpClient client, IConfiguration configuration)
        {
            this.client = client;

            settings = configuration.GetRequiredSection(nameof(Settings)).Get<Settings>();

        }

        public async Task<bool> Login(string username, string password)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

            var url = $"{settings.UrlBase}/Authentication/Login";

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

        //public async Task<bool> LoginGoogleService()
        //{
        //    client.DefaultRequestHeaders.Clear();
        //    //client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
        //    client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

        //    var url = $"{settings.UrlBase}/Authenticate/ExternalGoogle?provider=Google";

        //    //var loginRequest = "Google";

        //    //var json = JsonConvert.SerializeObject(loginRequest);
        //    //var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    //var response = await client.PostAsync(url, content);
        //    var response = await client.GetAsync(url);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var errorMessage = await response.Content.ReadAsStringAsync();
        //        throw new HttpRequestException($"Mensaje de error: {errorMessage}");
        //        //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
        //    }

        //    var resultado = await _mercadoPagoService.preparandoMP();

        //    var uri = new Uri(resultado[1]);
        //    await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

        //    var jsonResult = await response.Content.ReadAsStringAsync();

        //    var resultado = JsonConvert.DeserializeObject<UserResponse>(jsonResult);

        //    if (resultado != null && resultado.message.Contains("MSG_LOGIN_OK"))
        //    {
        //        //Preferences.Set("tokenGoingOut", resultado.tokenGoingOut);
        //        //Preferences.Set("IdUser", resultado.id);
        //        //Preferences.Set("userName", loginRequest.userName);
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public async Task<bool> Logout(string IdUser)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Authentication/Logout";

            var logoutRequest = new LogoutRequest
            {
                userId = IdUser
            };

            var json = JsonConvert.SerializeObject(logoutRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) return false;

            var jsonResult = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<LogoutResponse>(jsonResult);

            if (resultado != null && resultado.DESCRIPCION_DS.Contains("MSG_LOGOUT_OK"))
            {
                return true;
            }

            return false;


        }

        public async Task<bool> CreateUser(CreateUserRequest createUser)
        {
            if (createUser == null)
            {
                return false;
            }

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

            var url = $"{settings.UrlBase}/Authentication/CreateUser";


            var json = JsonConvert.SerializeObject(createUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) return false;

            var jsonResult = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<CreateUserRequest>(jsonResult);

            if (resultado != null && !String.IsNullOrEmpty(resultado.userName))
            {
                return true;
            }

            return false;


        }
        public async Task<string> ChangePassword(ChangePassRequest changePass)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);
            client.DefaultRequestHeaders.Add("Authorization", Preferences.Get("tokenGoingOut", string.Empty));

            var url = $"{settings.UrlBase}/Authentication/ChangePassword";

            var json = JsonConvert.SerializeObject(changePass);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var respuesta = "Usuario no encontrado o contraseña actual mal ingresada";
                return respuesta;
            }

            return "Ok";


        }
        public async Task<RecoverPasswordResponse> RecoverPassword(RecoverPassword recoverPassword)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

            var url = $"{settings.UrlBase}/Authentication/RecoverPassword";

            var recoverPasswordReq = new RecoverPassword
            {
                UserName = recoverPassword.UserName,
                Email = recoverPassword.Email
            };

            var json = JsonConvert.SerializeObject(recoverPasswordReq);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new RecoverPasswordResponse();
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RecoverPasswordResponse>(jsonResult);

        }
        public async Task<bool> RecoveryPassword(RecoveryPassword recoveryPassword)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("SecretKey", settings.SecretKey);
            client.DefaultRequestHeaders.Add("DbKey", settings.DbKey);

            var url = $"{settings.UrlBase}/Authentication/RecoveryPassword";

            var json = JsonConvert.SerializeObject(recoveryPassword);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Mensaje de error: {errorMessage}");
                //throw new HttpRequestException($"La solicitud HTTP no fue exitosa. Código de estado: {response.StatusCode}. Mensaje de error: {errorMessage}");
            }

            return true;

        }
    }

    public static class CustomAlertService
    {
        public static async Task<string> ShowCustomAlertAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            var page = new CustomAlertPage(tcs);
            await Shell.Current.Navigation.PushModalAsync(page);
            return await tcs.Task;
        }
    }

    internal class LoginGoogle
    {
        public string provider { get; set; }
    }
}
