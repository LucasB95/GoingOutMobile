using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public DateTime selectedDate = DateTime.Now;

        [ObservableProperty]
        public TimeSpan selectedTime;

        [ObservableProperty]
        public DateTime minDate = DateTime.Now;

        [ObservableProperty]
        public string observation;

        [ObservableProperty]
        private string idClient;

        [ObservableProperty]
        private string pageReturn;

        [ObservableProperty]
        private bool isActivity = false;


        private readonly IRestaurantService _restaurantService;
        private readonly INavegacionService _navegacionService;
        private readonly IMercadoPagoService _mercadoPagoService;

        public BookingsViewModel(IRestaurantService restaurantService, INavegacionService navegacionService, IMercadoPagoService mercadoPagoService)
        {
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
            _mercadoPagoService = mercadoPagoService;

            DateTime horaActual = DateTime.Now;
            TimeSpan tiempoAdicional = TimeSpan.FromMinutes(30);

            SelectedTime = horaActual.TimeOfDay + tiempoAdicional;
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IdClient = query["id"].ToString();
            PageReturn = query["page"].ToString();
        }

        [RelayCommand]
        async Task Reservar()
        {
            IsActivity = true;

            if (String.IsNullOrEmpty(CantidadSelected))
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccione la cantidad de personas", "Aceptar");
            }
            else if (SelectedDate.Date == DateTime.MinValue)
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccioné el dia para la reserva", "Aceptar");
            }
            else if (SelectedTime == TimeSpan.MinValue)
            {
                await Shell.Current.DisplayAlert("Mensaje", "Seleccioné el dia para la reserva", "Aceptar");
            }
            else if (!String.IsNullOrEmpty(CantidadSelected) && !String.IsNullOrEmpty(SelectedDate.ToString()))
            {

                DateTime fechaConHora = SelectedDate.Date + SelectedTime;

                if (fechaConHora < DateTime.Now)
                {
                    await Shell.Current.DisplayAlert("Mensaje", "Seleccioné correctamente la Hora para la reserva", "Aceptar");
                }
                else
                {

                    RestaurantResponse restaurant = await _restaurantService.DetailsRestaurant(IdClient);

                    BookingCreate bookingCreate = new BookingCreate()
                    {
                        AmountPeople = int.Parse(CantidadSelected),
                        DateReserve = fechaConHora,
                        UserId = Preferences.Get("IdUser", string.Empty),
                        ClientsId = IdClient,
                        DescriptionStateUser = Observation,
                        BusinessName = restaurant.BusinessName
                    };


                    var UserId = Preferences.Get("IdUser", string.Empty);
                    IEnumerable<Booking> bookings = await _restaurantService.GetBookings(UserId);

                    bool ValidoReserva = false;
                    if (bookings != null && bookings.Any())
                    {
                        ValidoReserva = bookings.Any(x => x.ClientsId == bookingCreate.ClientsId && x.UserId == bookingCreate.UserId && x.Date == bookingCreate.DateReserve);
                    }

                    if (ValidoReserva)
                    {
                        await Shell.Current.DisplayAlert("Mensaje", "Ya existe una reserva para ese dia y con esa hora", "Aceptar");
                    }
                    else
                    {
                        try
                        {

                            if (await _restaurantService.NewReserve(bookingCreate))
                            {
                                //Llevar a la lista de reservas desde el menu costadp
                                var uri = $"/{nameof(ReserveListPage)}";
                                await _navegacionService.GoToAsync(uri);

                            }
                            else
                            {
                                await Shell.Current.DisplayAlert("Mensaje", "No se pudo generar la reserva", "Aceptar");
                            }


                        }
                        catch (Exception e)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar");
                        }
                    }

                }

                //var resultado = await _mercadoPagoService.preparandoMP();
                //var uri = new Uri(resultado[1]);
                //await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

                //await Shell.Current.DisplayAlert("Mensaje", "sacar la cantidad desde el picker", "Aceptar");
            }

            IsActivity = false;
        }

        [RelayCommand]
        async Task GetBackEvent()
        {
            var uri = $"//{nameof(HomePage)}";
            if (PageReturn.Contains("RestaurantDetailPage"))
            {
                var category = Preferences.Get("nameCategory", string.Empty);
                uri = $"{nameof(RestaurantDetailPage)}?id={IdClient}&page={nameof(RestaurantListPage)}";
            }

            await _navegacionService.GoToAsync(uri);
        }

    }
}
