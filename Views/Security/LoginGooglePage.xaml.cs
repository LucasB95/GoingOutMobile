using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace GoingOutMobile.Views;

public partial class LoginGooglePage : ContentPage
{
    public LoginGooglePage()
    {
        InitializeComponent();

        // Realizar la solicitud POST para obtener la URL de autenticación
        PostToExternalGoogle();
        // Cargar la URL de autenticación externa en el WebView
        //string authUrl = "http://192.168.0.210/api/Authenticate/ExternalLogin?provider=Google";
        //webView.Source = authUrl;

        //// Manejar la navegación completada
        //webView.Navigated += WebView_Navigated;
    }
    private async void PostToExternalGoogle()
    {
        try
        {
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            var client = new HttpClient();
            var url = "http://192.168.0.210/api/Authenticate/ExternalGoogle?provider=Google";
            client.DefaultRequestHeaders.Add("DbKey", "GoingOutUsuarioBD");
            //var content = new StringContent(JsonConvert.SerializeObject(new { provider = "Google" }), Encoding.UTF8, "application/json");

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var htmlContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("HTML Content Received");
                webView.Source = new HtmlWebViewSource
                {
                    Html = htmlContent
                };
                webView.Navigated += WebView_Navigated;
                webView.Navigating += WebView_Navigating;
            }
            else
            {
                await DisplayAlert("Error", "Failed to initiate Google login.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
        }
    }

    private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        // For debugging purposes
        Debug.WriteLine($"Navigating to {e.Url}");
    }
    private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        if (e.Result == WebNavigationResult.Failure)
        {
            DisplayAlert("Error", "Failed to load content.", "OK");
            return;
        }
        if (e.Url.Contains("/Authenticate/AccesoExternoCallBack"))
        {
            // Procesar el callback de autenticación
            await ProcessAuthenticationCallback(e.Url);
        }
    }

    private async Task ProcessAuthenticationCallback(string url)
    {
        // Extraer el código de autenticación de la URL
        var uri = new Uri(url);
        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var code = queryParams["code"];

        if (!string.IsNullOrEmpty(code))
        {
            // Intercambiar el código por un token de acceso
            var token = await ExchangeCodeForTokenAsync(code);

            // Usar el token según sea necesario
            await DisplayAlert("Success", "Login successful", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Authentication failed", "OK");
        }
    }

    private async Task<string> ExchangeCodeForTokenAsync(string code)
    {
        using var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
        {
            Content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", "TU_CLIENT_ID"),
                    new KeyValuePair<string, string>("client_secret", "TU_CLIENT_SECRET"),
                    new KeyValuePair<string, string>("redirect_uri", "com.googleusercontent.apps.TU_CLIENT_ID:/oauth2redirect"),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                })
        };

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        //var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(payload);
        var tokenResponse = payload;

        return tokenResponse;
    }

}
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}