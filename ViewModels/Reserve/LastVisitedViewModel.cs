using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoingOutMobile.Models.Restaurant;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views.LastVisited;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels.Reserve
{
    public partial class LastVisitedViewModel : ViewModelGlobal, IQueryAttributable
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

        public LastVisitedViewModel(INavegacionService navegacionService, IGenericQueriesServices genericQueriesServices, IRestaurantService restaurantService)
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
                var uri = $"{nameof(LastVisitedDetailPage)}?id={ReserveSelected.ClientsId}&idBooking={ReserveSelected.Id}";
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
                    var lastVisited = listBooking.Where(x => x.BookingComplete == true).ToList();
                    if (lastVisited == null || lastVisited.Count == 0)
                    {
                        await Shell.Current.DisplayAlert("Mensaje", "No se pudo cargar la lista de reservas o no posee ninguna", "Aceptar");
                    }
                    else
                    {
                        ReserveCollection = new ObservableCollection<Booking>(lastVisited);
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
}
