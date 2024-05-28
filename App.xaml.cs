using GoingOutMobile.Views;

namespace GoingOutMobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Forzar el modo claro
        Application.Current.UserAppTheme = AppTheme.Light;

        MainPage = new AppShell();

    }
}
