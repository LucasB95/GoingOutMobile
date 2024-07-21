using GoingOutMobile.Services;
using GoingOutMobile.Views;
using GoingOutMobile.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using GoingOutMobile.Services.Interfaces;
using GoingOutMobile.Views.LastVisited;
using GoingOutMobile.ViewModels.Reserve;
using GoingOutMobile.Views.Security;
using GoingOutMobile.ViewModels.Security;
using CommunityToolkit.Maui;

namespace GoingOutMobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var assemblyInstance = Assembly.GetExecutingAssembly();
		using var stream = assemblyInstance.GetManifestResourceStream("GoingOutMobile.appsettings.json");

		var config = new ConfigurationBuilder()
			.AddJsonStream(stream)
			.Build();

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			}).UseMauiCommunityToolkit()
			.UseMauiMaps();

		builder.Configuration.AddConfiguration(config);

		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<INavegacionService, NavegacionService>();
		builder.Services.AddSingleton<IGenericQueriesServices, GenericQueriesServices>();
        builder.Services.AddSingleton<IRestaurantService, RestaurantService>();
        builder.Services.AddSingleton<SecurityService>();
        builder.Services.AddSingleton<IMaps, Maps>();
        //builder.Services.AddSingleton<IGoogleAuthService, GoogleAuthService>();

        builder.Services.AddTransient <MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();
        
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));


        builder.Services.AddSingleton<IMercadoPagoService, MercadoPagoService>();
        Routing.RegisterRoute(nameof(MercadoPagoPage), typeof(MercadoPagoPage));

        builder.Services.AddSingleton<BookingService>();

        //var app = builder.Build();

        //// Obtén una instancia de BookingCheckerService para iniciar el temporizador
        //var bookingCheckerService = app.Services.GetService<BookingService>();

        //return app;



        #region Security

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<CreateUserViewModel>();
        builder.Services.AddTransient<CreateUserPage>();

        builder.Services.AddTransient<ChangePasswordViewModel>();
        builder.Services.AddTransient<ChangePasswordPage>();

        builder.Services.AddTransient<RecoverPasswordViewModel>();
        builder.Services.AddTransient<RecoverPasswordPage>(); 
        
        builder.Services.AddTransient<RecoveryPasswordViewModel>();
        builder.Services.AddTransient<RecoveryPasswordPage>();

        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsPage>();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(LoginGooglePage), typeof(LoginGooglePage));
        Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
        Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
        Routing.RegisterRoute(nameof(RecoverPasswordPage), typeof(RecoverPasswordPage));
        Routing.RegisterRoute(nameof(RecoveryPasswordPage), typeof(RecoveryPasswordPage));

        #endregion

        #region Restaurant

        builder.Services.AddTransient<RestaurantListViewModel>();
        builder.Services.AddTransient<RestaurantListPage>();

        builder.Services.AddTransient<RestaurantDetailViewModel>();
        builder.Services.AddTransient<RestaurantDetailPage>();

        builder.Services.AddTransient<RestaurantFindListViewModel>();
        builder.Services.AddTransient<RestaurantFindListPage>();

        Routing.RegisterRoute(nameof(RestaurantListPage), typeof(RestaurantListPage));
        Routing.RegisterRoute(nameof(RestaurantDetailPage), typeof(RestaurantDetailPage));
        Routing.RegisterRoute(nameof(RestaurantFindListPage), typeof(RestaurantFindListPage));

        #endregion

        #region Reserve

        builder.Services.AddTransient<BookingsViewModel>();
        builder.Services.AddTransient<BookingsPage>();

        builder.Services.AddTransient<ReserveListViewModel>();
        builder.Services.AddTransient<ReserveListPage>();

        builder.Services.AddTransient<ReserveDetailViewModel>();
        builder.Services.AddTransient<ReserveDetailPage>();

        Routing.RegisterRoute(nameof(ReserveDetailPage), typeof(ReserveDetailPage));
        Routing.RegisterRoute(nameof(ReserveListPage), typeof(ReserveListPage));
        Routing.RegisterRoute(nameof(BookingsPage), typeof(BookingsPage));

        #endregion

        #region Varios

        builder.Services.AddTransient<LastVisitedViewModel>();
        builder.Services.AddTransient<LastVisitedPage>();

        builder.Services.AddTransient<LastVisitedDetailViewModel>();
        builder.Services.AddTransient<LastVisitedDetailPage>();

        builder.Services.AddTransient<FavoritesViewModel>();
        builder.Services.AddTransient<FavoritesPage>();      

        Routing.RegisterRoute(nameof(LastVisitedDetailPage), typeof(LastVisitedDetailPage));
        Routing.RegisterRoute(nameof(LastVisitedPage), typeof(LastVisitedPage));
        Routing.RegisterRoute(nameof(FavoritesPage), typeof(FavoritesPage));

        #endregion



#if DEBUG


        builder.Logging.AddDebug();
#endif


        var app = builder.Build();
        ServiceLocator.Initialize(app.Services);

        return app;

        //return builder.Build();
    }
}
