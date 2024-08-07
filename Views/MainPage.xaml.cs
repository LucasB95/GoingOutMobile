using GoingOutMobile.ViewModels;
using System.IdentityModel.Tokens.Jwt;

namespace GoingOutMobile.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        var accessToken = Preferences.Get("tokenGoingOut", string.Empty);

        if (accessToken != null && !String.IsNullOrEmpty(accessToken))
        {
            var jwt_token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var time = jwt_token.ValidTo;
            //var tokenUsuario = jwt_token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Rsa)?.Value;

            if (time < DateTime.UtcNow)
            {
                await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
            }

            (Shell.Current as AppShell).IsLogged = true;
        }
        else if (String.IsNullOrEmpty(accessToken))
        {
            (Shell.Current as AppShell).IsLogged = false;
            AppShell.ChangeStatus();
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }
    }
}