using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels
{
    public partial class BookingsViewModel : ViewModelGlobal, IQueryAttributable, INotifyPropertyChanged
    {
        [ObservableProperty]
        public string cantidadSelected;

        [ObservableProperty]
        public string observation;

        [ObservableProperty]
        private string idClient;


        private readonly IRestaurantService _restaurantService;
        private readonly INavegacionService _navegacionService;
        private readonly IMercadoPagoService _mercadoPagoService;

        public BookingsViewModel(IRestaurantService restaurantService, INavegacionService navegacionService, IMercadoPagoService mercadoPagoService)
        {
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
            _mercadoPagoService = mercadoPagoService;
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IdClient = query["id"].ToString();
        }



        [RelayCommand]
        async Task Reservar()
        {
            if (String.IsNullOrEmpty(CantidadSelected) && String.IsNullOrEmpty(Observation))
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccione la cantidad de personas", "Aceptar");
            }
            else if(!String.IsNullOrEmpty(CantidadSelected))
            {

                var resultado = await _mercadoPagoService.preparandoMP();
                var uri = new Uri(resultado[1]);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

                //await Shell.Current.DisplayAlert("Mensaje", "sacar la cantidad desde el picker", "Aceptar");
            }

        }

    }
}
