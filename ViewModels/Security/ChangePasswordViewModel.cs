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
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels.Security
{
    public partial class ChangePasswordViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string passwordConfirmation = string.Empty;

        [ObservableProperty]
        private string passwordConfirmation2 = string.Empty;

        [ObservableProperty]
        private bool isPassword = true;

        [ObservableProperty]
        private bool isPasswordConfirmation = true;

        [ObservableProperty]
        private bool isPasswordConfirmation2 = true;

        [ObservableProperty]
        private string iconSeePass = "eyeclose.svg";

        [ObservableProperty]
        private string iconSeePassConfirmation = "eyeclose.svg";

        [ObservableProperty]
        private string iconSeePassConfirmation2 = "eyeclose.svg";

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isActivity = false;

        private readonly SecurityService _securityService;
        private HomeViewModel _homeViewModel;

        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly INavegacionService _navegacionService;

        public INavegacionService NavegacionService => _navegacionService;

        public ChangePasswordViewModel(SecurityService securityService, IGenericQueriesServices genericQueriesServices, INavegacionService navegacionService, HomeViewModel homeViewModel)
        {
            _securityService = securityService;
            _genericQueriesServices = genericQueriesServices;
            _navegacionService = navegacionService;
            _homeViewModel = homeViewModel;

            //(Shell.Current as AppShell).IsLogged = false;
        }

        [RelayCommand]
        private async Task ChangePassMethod()
        {
            IsActivity = true;

            if (String.IsNullOrEmpty(Password))
            {
                await Shell.Current.DisplayAlert("Mensaje", "No ingreso la contraseña actual", "Aceptar");
            }
            else if (String.IsNullOrEmpty(PasswordConfirmation))
            {
                await Shell.Current.DisplayAlert("Mensaje", "No ingreso la nueva contraseña", "Aceptar");
            }
            else if (String.IsNullOrEmpty(PasswordConfirmation2))
            {
                await Shell.Current.DisplayAlert("Mensaje", "No ingreso la confirmacion de la nueva contraseña", "Aceptar");
            }
            else if (PasswordConfirmation != PasswordConfirmation2)
            {
                await Shell.Current.DisplayAlert("Mensaje", "La nueva contraseña no coincide con la repeticíon de contraseña", "Aceptar");
            }
            else
            {
                var changePassRequest = new ChangePassRequest
                {
                    UserName = Preferences.Get("userName", string.Empty),
                    Email = Preferences.Get("Email", string.Empty),
                    PasswordOld = Password,
                    PasswordNew = PasswordConfirmation
                };


                var resultado = await _securityService.ChangePassword(changePassRequest);

                if (resultado.Contains("Ok"))
                {
                    Name = Preferences.Get("userName", string.Empty);
                    var Persona = await _genericQueriesServices.GetInfoUser(Name);

                    if (Persona != null)
                    {
                        Preferences.Set("Email", Persona.Email);
                        Preferences.Set("UserId", Persona.UserId);

                        await Shell.Current.DisplayAlert("Mensaje", "Contraseña modificada correctamente", "Aceptar");

                        var uri = $"//{nameof(SettingsPage)}";
                        await _navegacionService.GoToAsync(uri);

                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", resultado, "Aceptar");
                }

            }

            IsActivity = false;
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
        public void ChangeStatusConfirmationPassword()
        {
            if (IsPasswordConfirmation)
            {
                IsPasswordConfirmation = false;
                IconSeePassConfirmation = "eyeopen.svg";
            }
            else
            {
                IsPasswordConfirmation = true;
                IconSeePassConfirmation = "eyeclose.svg";
            }
        }

        [RelayCommand]
        public void ChangeStatusConfirmation2Password()
        {
            if (IsPasswordConfirmation2)
            {
                IsPasswordConfirmation2 = false;
                IconSeePassConfirmation2 = "eyeopen.svg";
            }
            else
            {
                IsPasswordConfirmation2 = true;
                IconSeePassConfirmation2 = "eyeclose.svg";
            }
        }


    }

}
