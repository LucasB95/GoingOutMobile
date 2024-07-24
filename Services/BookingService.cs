using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Services
{
    public class BookingService : ViewModelGlobal
    {
        private readonly IRestaurantService _restaurantService;
        private readonly INavegacionService _navegacionService;
        private bool _userLoggedIn = false;
        private Timer _timer;
        public BookingService(IRestaurantService restaurantService, INavegacionService navegacionService)
        {
            _restaurantService = restaurantService;
            _navegacionService = navegacionService;
            StartTimer();
        }
        private void StartTimer()
        {
            _timer = new Timer(async _ => await CheckBookings(), null, TimeSpan.Zero, TimeSpan.FromSeconds(90));
        }

        public void UpdateUserLoginStatus(bool isLoggedIn)
        {
            _userLoggedIn = isLoggedIn;
        }

        private async Task CheckBookings()
        {
            if (!_userLoggedIn)
            {
                return; // No realizar comprobaciones si el usuario no está logueado
            }

            var UserId = Preferences.Get("IdUser", string.Empty);
            var listBooking = await _restaurantService.GetBookings(UserId);

            if (listBooking != null)
            {
                bool MostrarMGS = listBooking.Any(x => x.BookingComplete == false && x.Date > DateTime.Now);
                int ReservasPendientesAPI = listBooking
                    .Where(x => x.StateClient == false && String.IsNullOrEmpty(x.DescriptionStateClient) && x.Date > DateTime.Now)
                    .Count();

                int ReservasPendientes = !String.IsNullOrEmpty(Preferences.Get("ReservasPendientes", string.Empty))
                    ? int.Parse(Preferences.Get("ReservasPendientes", string.Empty))
                    : 0;

                if (ReservasPendientes == 0 && ReservasPendientesAPI > 0)
                {
                    Preferences.Set("ReservasPendientes", ReservasPendientesAPI.ToString());
                }

                ReservasPendientes = ReservasPendientes == 0 ? ReservasPendientesAPI : ReservasPendientes;

                bool cambiosDetectados = MostrarMGS && (ReservasPendientes > 0) && (ReservasPendientesAPI != ReservasPendientes);

                if (cambiosDetectados)
                {
                    ReservasMSG = true;
                    bool usuarioAcepto = await DisplayAlertOnMainThread("Reservas", "Revise el estado de sus reservas pendientes ya que se detecto respuesta. ¿Quiere verlas?", "Sí", "No");

                    if (usuarioAcepto)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                                var uri = $"//{nameof(ReserveListPage)}?page=HomePage";
                                await _navegacionService.GoToAsync(uri);
                        });

                    }
                }
            }
        }

        private async Task<bool> DisplayAlertOnMainThread(string title, string message, string accept, string cancel)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool result = await Shell.Current.DisplayAlert(title, message, accept, cancel);
                tcs.SetResult(result);
            });

            return await tcs.Task;
        }

    }

}
