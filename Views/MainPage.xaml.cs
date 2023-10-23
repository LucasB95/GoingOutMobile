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

        }
        if (String.IsNullOrEmpty(accessToken))
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }
    }
}