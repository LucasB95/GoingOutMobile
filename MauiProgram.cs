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
			});

		builder.Configuration.AddConfiguration(config);

		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton<HttpClient>();

		builder.Services.AddSingleton<INavegacionService, NavegacionService>();
		builder.Services.AddSingleton<IGenericQueriesServices, GenericQueriesServices>();
		builder.Services.AddSingleton<SecurityService>();
		builder.Services.AddSingleton<SecurityService>();
		builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient <MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();


        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
