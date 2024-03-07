using GoingOutMobile.Services;
using GoingOutMobile.Views;
using GoingOutMobile.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using GoingOutMobile.Services.Interfaces;

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
			})
			.UseMauiMaps();

		builder.Configuration.AddConfiguration(config);

		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton<HttpClient>();

		builder.Services.AddSingleton<INavegacionService, NavegacionService>();
		builder.Services.AddSingleton<IGenericQueriesServices, GenericQueriesServices>();
        builder.Services.AddSingleton<IRestaurantService, RestaurantService>();
        builder.Services.AddSingleton<IMercadoPagoService, MercadoPagoService>();

        builder.Services.AddSingleton<SecurityService>();

		builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<CreateUserViewModel>();
        builder.Services.AddTransient<CreateUserPage>();

        builder.Services.AddTransient <MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsPage>();

        builder.Services.AddTransient<RestaurantListViewModel>();
        builder.Services.AddTransient<RestaurantListPage>();

        builder.Services.AddTransient<RestaurantDetailViewModel>();
        builder.Services.AddTransient<RestaurantDetailPage>();

        builder.Services.AddTransient<BookingsViewModel>();
        builder.Services.AddTransient<BookingsPage>();

        builder.Services.AddTransient<RestaurantFindListViewModel>();
        builder.Services.AddTransient<RestaurantFindListPage>();

        Routing.RegisterRoute(nameof(MercadoPagoPage), typeof(MercadoPagoPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));

        Routing.RegisterRoute(nameof(RestaurantListPage), typeof(RestaurantListPage));
        Routing.RegisterRoute(nameof(RestaurantDetailPage), typeof(RestaurantDetailPage));
        Routing.RegisterRoute(nameof(RestaurantFindListPage), typeof(RestaurantFindListPage));
        Routing.RegisterRoute(nameof(BookingsPage), typeof(BookingsPage));

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
