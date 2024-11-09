using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using CAPS.Services;
using CAPS.Views;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using CAPS.Views.Components;
using CAPS.Views.WaypointList;
using CAPS.Services.Mission;
using CAPS.Services.Geo;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices.JavaScript;

namespace CAPS;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	#region Initalization and Constructor

	public static IHost AppHost { get; private set; } = default!;

	public App()
	{
		// Build configuration
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		// Configure Serilog
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();

		// Build the App Host
		AppHost = Host.CreateDefaultBuilder()
			.ConfigureServices((context, services) => InitializeServices(services))
			.Build();
	}

	protected override async void OnStartup(StartupEventArgs e)
	{
		await AppHost.StartAsync();

		base.OnStartup(e);

		var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();

		mainWindow.Show();
	}

	protected override async void OnExit(ExitEventArgs e)
	{
		await AppHost.StopAsync();
		AppHost.Dispose();

		base.OnExit(e);
	}

	#endregion

	private void InitializeServices(IServiceCollection services)
	{
		services.AddSingleton<IMyService, MyService>();
		services.AddSingleton<IMissionManager, MissionManager>();
		services.AddSingleton<ITheaterService, TheaterService>();
		services.AddSingleton<ICoordinateConverterService, CoordinateConverterService>();

		services.AddSingleton<MainWindow>();
		services.AddSingleton<MainWindowViewModel>();
		services.AddTransient<WaypointList>();
		services.AddTransient<WaypointListViewModel>();
		services.AddTransient<TreeViewItemViewModel>();

		// Register Serilog
		services.AddLogging(configure => configure.AddSerilog());
	}

	#region Extension Methods

	/// <summary>
	/// Gets registered service.
	/// </summary>
	/// <typeparam name="T">Type of the service to get.</typeparam>
	/// <returns>Instance of the service or <see langword="null"/>.</returns>
	public static T GetService<T>()
		where T : class
	{
		var service = AppHost.Services.GetService(typeof(T)) as T;
		return service!;

	}

	#endregion
}
