using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using CAPS.Services;
using CAPS.Views;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using CAPS.Views.Components;
using CAPS.Views.WaypointList;

namespace CAPS
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region Initalization and Constructor

		private IServiceProvider _serviceProvider;

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

			var serviceCollection = new ServiceCollection();

			ConfigureServices(serviceCollection);
			ConfigureViewModels(serviceCollection);

			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
			mainWindow.Show();
		}

		#endregion

		private void ConfigureServices(ServiceCollection services)
		{
			// Register Serilog
			services.AddLogging(configure => configure.AddSerilog());

			// Register your services here
			services.AddTransient<IMyService, MyService>();
		}

		private void ConfigureViewModels(ServiceCollection services)
		{
			// Register your view models here
			services.AddTransient<MainWindowViewModel>();
			services.AddTransient<TreeViewItemViewModel>();
			services.AddTransient<WaypointListViewModel>();

			// Register the views
			services.AddTransient<WaypointList>();

			// Register the main window
			services.AddSingleton<MainWindow>();
		}
	}
}
