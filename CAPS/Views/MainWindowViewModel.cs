using CAPS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CAPS.Views;

internal class MainWindowViewModel: BaseViewModel
{
	private readonly IMyService _myService;

	private string _title = string.Empty;

	public string Title
	{
		get => _title;
		set
		{
			_title = value;
			OnPropertyChanged();
		}
	}

	public MainWindowViewModel(IMyService myService)
	{
		_myService = myService;
		_myService.DoWork();

		// Retrieve the version information
		var version = Assembly.GetExecutingAssembly().GetName().Version;
		Title = $"CAPS - Version {version}";
	}
}
