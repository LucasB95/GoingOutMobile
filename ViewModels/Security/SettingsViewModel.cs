using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using GoingOutMobile.Views.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.Communication;

namespace GoingOutMobile.ViewModels
{
    public partial class SettingsViewModel : ViewModelGlobal
    {

        [ObservableProperty]
        string nombreUsuario;

        [ObservableProperty]
        string correoelectronico;

        private readonly INavegacionService _navegacionService;
        private readonly SecurityService _securityService;


        public SettingsViewModel(INavegacionService navegacionService, SecurityService securityService)
        {
            _navegacionService = navegacionService;
            _securityService = securityService;
            NombreUsuario = Preferences.Get("userName", string.Empty);
            Correoelectronico = Preferences.Get("Email", string.Empty);
        }

        [RelayCommand]
        async Task ContactSupport()
        {
            try
            {
                if (Email.Default.IsComposeSupported)
                {

                    string subject = "Ayuda/Consulta";
                    string body = "Necesito contactarme con alguien de Soporte. \n " +
                                  "Escribe tu consulta: \n";
                    string[] recipients = new[] { "soportegoingout@gmail.com"};

                    var message = new EmailMessage
                    {
                        Subject = subject,
                        Body = body,
                        BodyFormat = EmailBodyFormat.PlainText,
                        To = new List<string>(recipients)
                    };

                    await Email.Default.ComposeAsync(message);
                }
            }
            catch (FeatureNotSupportedException ex)
            {
                // Email is not supported on this device
                await Shell.Current.DisplayAlert("Error", "Email is not supported on this device.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Mensaje", "No ingreso la nueva contraseña", "Aceptar");
                await Shell.Current.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }

        }

        [RelayCommand]
        async Task ChangePass()
        {           
            var uri = $"{nameof(ChangePasswordPage)}";
            await _navegacionService.GoToAsync(uri);

        }
        
        [RelayCommand]
        async Task SalirSesion()
        {
            var IdUser = Preferences.Get("IdUser", string.Empty);
            await _securityService.Logout(IdUser);
            Preferences.Set("tokenGoingOut", string.Empty);
            ReservasMSG = false;
            var uri = $"{nameof(HomePage)}";
            await _navegacionService.GoToAsync(uri);

        }
    }
}
