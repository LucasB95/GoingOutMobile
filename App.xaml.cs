using GoingOutMobile.Views;

namespace GoingOutMobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

    }
}
