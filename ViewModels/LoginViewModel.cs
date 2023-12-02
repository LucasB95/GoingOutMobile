using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels
{
    public partial class LoginViewModel : ViewModelGlobal
    {
        private readonly IConnectivity _connectivity;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isPassword = true;

        [ObservableProperty]
        private string iconSeePass = "eyeclose.svg";

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isActivity = false;

        private readonly SecurityService _securityService;
        private HomeViewModel _homeViewModel;

        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly INavegacionService _navegacionService;

        public LoginViewModel(IConnectivity connectivity, SecurityService securityService, IGenericQueriesServices genericQueriesServices, INavegacionService navegacionService, HomeViewModel homeViewModel)
        {
            _connectivity = connectivity;
            _securityService = securityService;
            _connectivity.ConnectivityChanged += _connectivity_ConnectivityChanged;
            _genericQueriesServices = genericQueriesServices;
            _navegacionService = navegacionService;
            _homeViewModel = homeViewModel;

            (Shell.Current as AppShell).IsLogged = false;
        }

        private void _connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            LoginMethodCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(StatusConnection))]
        private async Task LoginMethod()
        {
            IsActivity = true;

            var resultado = await _securityService.Login(UserName, Password);

            if (resultado)
            {
                Name = Preferences.Get("userName", string.Empty);
                var Persona = await _genericQueriesServices.GetInfoUser(Name);

                if (Persona != null)
                {
                    Preferences.Set("Email", Persona.Email);
                    Preferences.Set("UserId", Persona.UserId);

                    Application.Current.MainPage = new AppShell();
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Mensaje", "Ingreso credenciales erroneas", "Aceptar");
            }

            IsActivity = false;
        }
        private bool StatusConnection()
        {
            return _connectivity.NetworkAccess == NetworkAccess.Internet ? true : false;
        }

        [RelayCommand]
        public void ChangeStatusPassword()
        {
            if (IsPassword)
            {
                IsPassword = false;
                IconSeePass = "eyeopen.svg";
            }
            else
            {
                IsPassword = true;
                IconSeePass = "eyeclose.svg";
            }
        }

        [RelayCommand]
        async Task CreateUser()
        {
            var uri = $"{nameof(CreateUserPage)}";
            await _navegacionService.GoToAsync(uri);
        }

    }
}
