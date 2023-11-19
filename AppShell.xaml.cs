using CommunityToolkit.Mvvm.ComponentModel;
using GoingOutMobile.ViewModels;
using GoingOutMobile.Views;
using Microsoft.Maui.Controls;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;

namespace GoingOutMobile;

public partial class AppShell : Shell
{

    private static bool chequed = false;

    private static bool isVisible = false;
    public AppShell()
    {
        InitializeComponent();
    }

    public bool IsLogged
    {
        get => (bool)GetValue(IsLoggedProperty);
        set => SetValue(IsLoggedProperty, value);
    }

    public static readonly BindableProperty IsLoggedProperty =
    BindableProperty.Create("IsLogged", typeof(bool), typeof(AppShell), false, propertyChanged: IsLogged_PropertyChanged);

    private static void IsLogged_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        chequed = (bool)newValue ? true : false;
        ChangeStatus();
    }

    public static void ChangeStatus()
    {
        Shell.Current.FlyoutBehavior = chequed ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
    }

}
