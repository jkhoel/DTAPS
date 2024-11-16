using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Library.Models.Dcs.Modules.Oh58;
using Planner.Core.Class;
using Planner.Framework.Manager;
using Planner.Framework.ViewModel.Waypoint;
using System.Collections.ObjectModel;

namespace Planner.Framework.ViewModel.Radio;

public partial class RadioListItemViewModel(double frequency, string name) : ObservableObject
{
	[ObservableProperty]
	public double frequency = frequency;

	[ObservableProperty]
	public string name = name;
}

public partial class RadioListViewModel : ViewModelBase
{
	private readonly ILogger<RadioListViewModel> _logger;
	private readonly IMissionManager _missionManager;

	[ObservableProperty]
	public ObservableCollection<RadioListItemViewModel> uamRadioItems = [];

	[ObservableProperty]
	public ObservableCollection<RadioListItemViewModel> vamRadioItems = [];

	[ObservableProperty]
	public ObservableCollection<RadioListItemViewModel> vfm1RadioItems = [];

	[ObservableProperty]
	public ObservableCollection<RadioListItemViewModel> vfm2RadioItems = [];

	public RadioListViewModel(ILogger<RadioListViewModel> logger, IMissionManager missionManager)
	{
		_logger = logger;
		_missionManager = missionManager;

		// Subscribe to the PropertyChanged event of _missionManager
		_missionManager.ActiveMissionChanged += OnActiveMissionChanged;

		InitializeRadio();
	}

	private void InitializeRadio()
	{
		if (_missionManager.ActiveMission.Radios is not { } radios)
			return;

		UamRadioItems = AddRadioItems(radios.Uam);
		VamRadioItems = AddRadioItems(radios.Vam);
		Vfm1RadioItems = AddRadioItems(radios.Vfm1);
		Vfm2RadioItems = AddRadioItems(radios.Vfm2);
	}

	private static ObservableCollection<RadioListItemViewModel> AddRadioItems(List<RadioPreset>? radioPresets)
	{
		if (radioPresets == null) return [];

		var items = new ObservableCollection<RadioListItemViewModel>();

		foreach (var radio in radioPresets)
		{
			var radioItem = new RadioListItemViewModel(radio.Frequency, radio.Name);
			items.Add(radioItem);
		}

		return items;
	}

	private void OnActiveMissionChanged(object? sender, MissionFile newActiveMission)
	{
		InitializeRadio();
	}

	[RelayCommand]
	public void UpdateRadio()
	{
		//_missionManager.UpdateActiveMissionRadio(Radio);
	}
}
