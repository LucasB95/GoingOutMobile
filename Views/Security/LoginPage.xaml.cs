using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        BindingContext = loginViewModel;
    }
}