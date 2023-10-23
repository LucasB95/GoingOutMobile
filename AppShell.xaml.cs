using CommunityToolkit.Mvvm.ComponentModel;
using GoingOutMobile.ViewModels;
using GoingOutMobile.Views;

namespace GoingOutMobile;

public partial class AppShell : Shell
{   
    public string probando;
    public AppShell()
    {
        InitializeComponent();

    }
    protected override async void OnAppearing()
    {
        var probando = Preferences.Get("Name", string.Empty);
    }

        private async void CerrarSesion(object sender, EventArgs e)
    {
        Preferences.Set("tokenGoingOut", string.Empty);
        var uri = $"//{nameof(MainPage)}";
        await Shell.Current.GoToAsync(uri);
    }
}
