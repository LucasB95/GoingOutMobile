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
    public partial class SettingsViewModel : ViewModelGlobal
    {

        [ObservableProperty]
        string nombreUsuario;

        [ObservableProperty]
        string email;

        private readonly INavegacionService _navegacionService;
        private readonly SecurityService _securityService;


        public SettingsViewModel(INavegacionService navegacionService, SecurityService securityService)
        {
            _navegacionService = navegacionService;
            _securityService = securityService;
            NombreUsuario = Preferences.Get("userName", string.Empty);
            Email = Preferences.Get("Email", string.Empty);
        }

        [RelayCommand]
        async Task SalirSesion()
        {
            var IdUser = Preferences.Get("IdUser", string.Empty);
            await _securityService.Logout(IdUser);
            Preferences.Set("tokenGoingOut", string.Empty);
            var uri = $"{nameof(HomePage)}";
            await _navegacionService.GoToAsync(uri);

        }
    }
}
