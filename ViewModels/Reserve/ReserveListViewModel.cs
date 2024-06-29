using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using MercadoPago.Resource.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels
{
    public partial class ReserveListViewModel : ViewModelGlobal, IQueryAttributable
    {
        [ObservableProperty]
        Booking reserveSelected;

        [ObservableProperty]
        ObservableCollection<Booking> reserveCollection;

        [ObservableProperty]
        private bool isActivity = false;

        [ObservableProperty]
        bool isRefreshing;

        public Command GetDataCommand { get; set; }

        private readonly INavegacionService _navegacionService;
        private readonly IGenericQueriesServices _genericQueriesServices;
        private readonly IRestaurantService _restaurantService;

        public ReserveListViewModel(INavegacionService navegacionService, IGenericQueriesServices genericQueriesServices, IRestaurantService restaurantService)
        {
            _navegacionService = navegacionService;
            _genericQueriesServices = genericQueriesServices;
            _restaurantService = restaurantService;

            GetDataCommand = new Command(async () => await Refresh());
            GetDataCommand.Execute(this);
            PropertyChanged += ReserveListViewModel_PropertyChanged;
        }

        private async void ReserveListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReserveSelected))
            {
                var uri = $"{nameof(ReserveDetailPage)}?id={ReserveSelected.ClientsId}&idBooking={ReserveSelected.Id}";
                await _navegacionService.GoToAsync(uri);
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var page = query["page"].ToString();

            if (!String.IsNullOrEmpty(page))
            {
                IsRefreshing = true;
                await RefreshCommand.ExecuteAsync(this);
            }
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var UserId = Preferences.Get("IdUser", string.Empty);
                var listBooking = await _restaurantService.GetBookings(UserId);
                if (listBooking != null)
                {
                    var reserveList = listBooking.Where(x => x.BookingComplete == false).ToList();
                    if (reserveList != null && reserveList.Count > 0)
                    {
                        ReserveCollection = new ObservableCollection<Booking>(reserveList.OrderBy(x => x.Date));

                        int ReservasPendientes = reserveList.Where(x => x.StateClient == false && String.IsNullOrEmpty(x.DescriptionStateClient)).ToList().Count();

                        if (ReservasPendientes > 0)
                        {
                            Preferences.Set("ReservasPendientes", ReservasPendientes.ToString());
                        }
                        else
                        {
                            Preferences.Set("ReservasPendientes", string.Empty);
                        }

                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Mensaje", "No posee Reservas activas", "Aceptar");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Mensaje", "No se pudo cargar la lista de reservas o no posee ninguna", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task Refresh()
        {
            GetDataCommand = new Command(async () => await LoadDataAsync());
            GetDataCommand.Execute(this);

            IsRefreshing = false;
        }
    }
    public class StateClientToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Booking booking)
            {
                if (!booking.StateClient)
                {
                    return string.IsNullOrEmpty(booking.DescriptionStateClient) ? "Esperando confirmación" : "Reserva Rechazada";
                }
                else
                {
                    return "Reserva Confirmada";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
