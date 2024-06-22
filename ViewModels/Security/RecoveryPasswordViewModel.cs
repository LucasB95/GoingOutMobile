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
    public partial class RecoveryPasswordViewModel : ViewModelGlobal, IQueryAttributable
    {
        [ObservableProperty]
        private string codeVerification;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string passwordConfirmation;

        [ObservableProperty]
        private string passwordConfirmation2;

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

        public RecoveryPasswordViewModel(SecurityService securityService, IGenericQueriesServices genericQueriesServices, INavegacionService navegacionService, HomeViewModel homeViewModel)
        {
            _securityService = securityService;
            _genericQueriesServices = genericQueriesServices;
            _navegacionService = navegacionService;
            _homeViewModel = homeViewModel;

            (Shell.Current as AppShell).IsLogged = false;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CodeVerification = query["id"].ToString();
        }

        [RelayCommand]
        private async Task ChangePassMethod()
        {
            IsActivity = true;

            if (String.IsNullOrEmpty(PasswordConfirmation))
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

                var recoveryPassword = new RecoveryPassword
                {
                    Id = Guid.Parse(Preferences.Get("IdRecoverPass", string.Empty)),
                    DateChange = DateTime.Now,
                    Password = PasswordConfirmation,
                    CodeGenerate = int.Parse(CodeVerification),
                    UserName = Preferences.Get("userName", string.Empty),
                    Email = Preferences.Get("Email", string.Empty)
                };

                var resultado = await _securityService.RecoveryPassword(recoveryPassword);

                if (resultado)
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Contraseña modificada correctamente", "Aceptar");

                    var uri = $"/{nameof(LoginPage)}";
                    await _navegacionService.GoToAsync(uri);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Fallo el recupero de contraseña, reintente mas tarde", "Aceptar");
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
            if (IsPasswordConfirmation)
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
