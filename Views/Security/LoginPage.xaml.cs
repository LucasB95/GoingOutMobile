using GoingOutMobile.GoogleAuth;
using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly LoginViewModel _loginViewModel;
    public LoginPage(LoginViewModel loginViewModel, IGoogleAuthService googleAuthService)
    {
        InitializeComponent();
        BindingContext = loginViewModel;
        _googleAuthService = googleAuthService;
        _loginViewModel = loginViewModel;
    }

    private async void LoginGoogle_Clicked(object sender, EventArgs e)
    {

        //var loggedUser = await _googleAuthService.GetCurrentUserAsync();

        //if (loggedUser == null)
        //{
        var loggedUser = await _googleAuthService.AuthenticateAsync();
        //}

        if (loggedUser != null)
        {
            Preferences.Set("userName", loggedUser.FullName);
            Preferences.Set("Email", loggedUser.Email);
            await _loginViewModel.LoginGoogleMethod(loggedUser.TokenId);
        }

        //await Application.Current.MainPage.DisplayAlert("Login Message", "Welcome " + loggedUser.FullName, "Ok");

    }


}