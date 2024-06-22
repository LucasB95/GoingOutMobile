using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Login;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GoingOutMobile.ViewModels.Security
{
    public partial class RecoverPasswordViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isActivity = false;

        private readonly SecurityService _securityService;
        private HomeViewModel _homeViewModel;

        private readonly IConnectivity _connectivity;
        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly INavegacionService _navegacionService;

        public RecoverPasswordViewModel(IConnectivity connectivity, SecurityService securityService, IGenericQueriesServices genericQueriesServices, INavegacionService navegacionService, HomeViewModel homeViewModel)
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
            RecoverPassCommand.NotifyCanExecuteChanged();
        }
        private bool StatusConnection()
        {
            return _connectivity.NetworkAccess == NetworkAccess.Internet ? true : false;
        }

        [RelayCommand(CanExecute = nameof(StatusConnection))]
        private async Task RecoverPass()
        {
            IsActivity = true;

            RecoverPassword recoverPassword = new RecoverPassword()
            {
                UserName = this.UserName,
                Email = this.Email,
            };

            var resultado = await _securityService.RecoverPassword(recoverPassword);

            if (resultado != null || !String.IsNullOrEmpty(resultado.UserId))
            {

                Preferences.Set("CodeGenerate", resultado.CodeGenerate);
                Preferences.Set("IdRecoverPass", resultado.Id.ToString());
                Preferences.Set("RecoverPassDateTime", resultado.DateRecover.ToString());
                Preferences.Set("userName", resultado.UserName);
                Preferences.Set("Email", resultado.Email);

                var result = await CustomAlertService.ShowCustomAlertAsync();
                if (result != null && resultado.CodeGenerate == result)
                {
                    var uri = $"{nameof(RecoveryPasswordPage)}?id={result}";
                    await _navegacionService.GoToAsync(uri);;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Se termino el tiempo de espera para ingresar el código enviado al email", "Aceptar");
                }

            }
            else
            {
                await Shell.Current.DisplayAlert("Mensaje", "Usuario no encontrado, Email o Usuario incorrectos", "Aceptar");
            }

            IsActivity = false;
        }        
        
        [RelayCommand(CanExecute = nameof(StatusConnection))]
        private async Task RecoverPassModal()
        {
            IsActivity = true;

            string savedCodeGenerate = Preferences.Get("CodeGenerate", string.Empty);
            string dateRecover = Preferences.Get("CodeGenerate", string.Empty);

            if (!String.IsNullOrEmpty(savedCodeGenerate))
            {
                var result = await CustomAlertService.ShowCustomAlertAsync();
                if (result != null && savedCodeGenerate == result)
                {
                    var uri = $"{nameof(RecoveryPasswordPage)}";
                    await _navegacionService.GoToAsync(uri);;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Se termino el tiempo de espera para ingresar el código enviado al email", "Aceptar");
                }

            }
            else
            {
                await Shell.Current.DisplayAlert("Mensaje", "Genere una nueva solicitud de Recuperar Contraseña", "Aceptar");
            }

            IsActivity = false;
        }


    }
}
