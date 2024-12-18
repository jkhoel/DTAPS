using CAPS.Core;
using CAPS.Services.Mission;
using CAPS.ViewModel.Framework;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reflection;

namespace CAPS.ViewModel;

public partial class MainWindowViewModel : ViewModelBase
{
	public ObservableCollection<TreeViewItemViewModel> TreeViewItems { get; }

	[ObservableProperty]
	private string title = string.Empty;

	private TreeViewItemViewModel selectedItem;

	private readonly IMissionManager _missionManager;

	public TreeViewItemViewModel SelectedItem
	{
		get => selectedItem;
		set
		{
			if (SetProperty(ref selectedItem, value))
			{
				selectedItem?.OnSelectCommand.Execute(null);
			}
		}
	}


	public MainWindowViewModel(IMissionManager missionManager)
	{
		_missionManager = missionManager;

		// Retrieve the version information
		var version = Assembly.GetExecutingAssembly().GetName().Version;
		Title = $"CAPS - Version {version}";

		// Initialize TreeView items
		TreeViewItems =
		[
			new("Mission 1",
			[
				new TreeViewItemViewModel("Settings", null, OnMissionSelected),
				new TreeViewItemViewModel("Laser Codes", null, OnMissionSelected),
				new TreeViewItemViewModel("Notebook", null, OnMissionSelected),
				new TreeViewItemViewModel("Prepoints", null, OnMissionSelected),
				new TreeViewItemViewModel("Radios", null, OnMissionSelected),
				new TreeViewItemViewModel("Routes", null, OnMissionSelected),
				new TreeViewItemViewModel("TargetPoints", null, OnMissionSelected),
				new TreeViewItemViewModel("Waypoints", null, OnMissionSelected),
			], OnMissionSelected),
			//new("Mission 2",
			//[
			//	new TreeViewItemViewModel("Options", null, OnMissionSelected),
			//	new TreeViewItemViewModel("Waypoints", null, OnMissionSelected),
			//	new TreeViewItemViewModel("Other", null, OnMissionSelected)
			//], OnMissionSelected),
			//new("Mission 3",
			//[
			//	new TreeViewItemViewModel("Options", null, OnMissionSelected),
			//	new TreeViewItemViewModel("Waypoints", null, OnMissionSelected),
			//	new TreeViewItemViewModel("Other", null, OnMissionSelected)
			//], OnMissionSelected),
			//new("Maps",
			//[
			//	new TreeViewItemViewModel("Map 1",
			//	[
			//		new TreeViewItemViewModel("Layer 2", null, OnMissionSelected),
			//		new TreeViewItemViewModel("Layer 1", null, OnMissionSelected),
			//		new TreeViewItemViewModel("Base Layer", null, OnMissionSelected)
			//	]),
			//	new TreeViewItemViewModel("Map 2",
			//	[
			//		new TreeViewItemViewModel("Layer 2", null, OnMissionSelected),
			//		new TreeViewItemViewModel("Layer 1", null, OnMissionSelected),
			//		new TreeViewItemViewModel("Base Layer", null, OnMissionSelected)
			//	])
			//],OnMissionSelected)
		];
	}

	public RelayCommand<TreeViewItemViewModel> OnMissionSelectedCommand => new RelayCommand<TreeViewItemViewModel>(OnMissionSelected);

	private void OnMissionSelected(TreeViewItemViewModel? model)
	{
		if (model is TreeViewItemViewModel item)
			SelectedItem = item;
	}

	[RelayCommand]
	public async Task ImportMission()
	{
		await _missionManager.LoadMissionFile();
	}

	[RelayCommand]
	public async Task ExportMission()
	{
		await _missionManager.ExportActiveMission();
	}
}
