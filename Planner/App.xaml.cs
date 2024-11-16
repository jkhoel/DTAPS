using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Serilog;
using System.IO;
using Planner.Core.Class;
using Planner.Core.Service;
using Planner.Framework.View;
using Planner.Framework.ViewModel;
using Planner.Framework.Manager;
using Planner.Framework.ViewModel.PlannerTree;
using Planner.Framework.View.Waypoint;
using Planner.Framework.ViewModel.Waypoint;
using Planner.Framework.View.Radio;
using Planner.Framework.ViewModel.Radio;
using Planner.Framework.ViewModel.MissionSettings;
using Planner.Framework.View.MissionSettings;

namespace Planner;

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
		services.AddSingleton<IMissionManager, MissionManager>();
		services.AddSingleton<ITheaterService, TheaterService>();
		services.AddSingleton<INavigationService, NavigationService>();
		services.AddSingleton<ICoordinateSystemService, CoordinateSystemService>();

		services.AddSingleton<MainWindow>();
		services.AddSingleton<MainWindowViewModel>();

		services.AddTransient<TreeItemViewModel>();

		services.AddTransient<MissionSettingsView>();
		services.AddTransient<MissionSettingsViewModel>();

		services.AddTransient<WaypointListView>();
		services.AddTransient<WaypointListViewModel>();

		services.AddTransient<RadioListView>();
		services.AddTransient<RadioListViewModel>();



		// Register delegates
		services.AddSingleton<Func<Type, ViewModelBase>>(provider => viewModelType => (ViewModelBase)provider.GetRequiredService(viewModelType));

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
