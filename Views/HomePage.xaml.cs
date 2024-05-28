using GoingOutMobile.ViewModels;
using System.IdentityModel.Tokens.Jwt;

namespace GoingOutMobile.Views;

public partial class HomePage : ContentPage
{
    HomeViewModel _homeViewModel;
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _homeViewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        var accessToken = Preferences.Get("tokenGoingOut", string.Empty);

        if (accessToken != null && !String.IsNullOrEmpty(accessToken))
        {
            var jwt_token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var time = jwt_token.ValidTo;
            //var tokenUsuario = jwt_token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Rsa)?.Value;
            var tiempoToken = DateTime.Compare(time, DateTime.UtcNow);
            if (time < DateTime.UtcNow)
            {
                await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
            }
            else
            {
                await _homeViewModel.RefreshCommand.ExecuteAsync(this);

                bool isloggedControl = (Shell.Current as AppShell).IsLogged;

                if (!isloggedControl)
                {
                    (Shell.Current as AppShell).IsLogged = true;
                }

            }

        }
        else if (String.IsNullOrEmpty(accessToken))
        {
            (Shell.Current as AppShell).IsLogged = false;
            AppShell.ChangeStatus();
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }
    }

}