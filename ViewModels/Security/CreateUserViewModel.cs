using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Login;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels
{
    public partial class CreateUserViewModel : ViewModelGlobal
    {
        private readonly IConnectivity _connectivity;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private bool isPassword = true;

        [ObservableProperty]
        private string iconSeePass = "eyeclose.svg";


        private readonly SecurityService _securityService;
        private readonly INavegacionService _navegacionService;

        public CreateUserViewModel(SecurityService securityService, IConnectivity connectivity, INavegacionService navegacionService)
        {
            _securityService = securityService;
            _connectivity = connectivity;
            _connectivity.ConnectivityChanged += _connectivity_ConnectivityChanged;
            _navegacionService = navegacionService;
        }
        private void _connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            CreateUserCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(StatusConnection))]
        private async Task CreateUser()
        {
            if (String.IsNullOrEmpty(Name) &&
                String.IsNullOrEmpty(Email) &&
                String.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Faltan campos por completar", "Aceptar");
            }
            else
            {
                Regex reg = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);

                Match match = reg.Match(Email);

                if (!match.Success)
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Ingrese un e-mail valido", "Aceptar");
                }
                else
                {
                    CreateUserRequest createUser = new CreateUserRequest
                    {
                        name = Name,
                        email = Email,
                        pass = Password,
                        userName = String.IsNullOrEmpty(UserName) ? "" : UserName,
                    };

                    var resultado = await _securityService.CreateUser(createUser);

                    if (resultado)
                    {
                        await Shell.Current.DisplayAlert("Mensaje", "El usuario " + createUser.userName + "se creo correctamente", "Aceptar");
                        Application.Current.MainPage = new AppShell();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Mensaje", "Ocurrio un error en la creacion del usuario, vuelva a intentar", "Aceptar");
                    }
                }
            }

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
        async Task GetBackEvent()
        {
            var uri = $"{nameof(LoginPage)}";
            await _navegacionService.GoToAsync(uri);
        }
    }
}
