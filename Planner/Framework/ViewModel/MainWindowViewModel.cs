using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planner.Core.Class;
using Planner.Core.Service;
using Planner.Framework.Manager;
using Planner.Framework.ViewModel.PlannerTree;
using Planner.Framework.ViewModel.Radio;
using Planner.Framework.ViewModel.Waypoint;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Planner.Framework.ViewModel;

public partial class MainWindowViewModel : ViewModelBase
{
	public ObservableCollection<TreeItemViewModel> TreeViewItems { get; }

	[ObservableProperty]
	private string title = string.Empty;

	[ObservableProperty]
	private TreeItemViewModel selectedItem = default!;

	[ObservableProperty]
	private INavigationService navigationService;

	[ObservableProperty]
	private IMissionManager missionManager;

	public MainWindowViewModel(IMissionManager missionManager, INavigationService navigationService)
	{
		MissionManager = missionManager;
		NavigationService = navigationService;

		// Retrieve the version information
		var version = Assembly.GetExecutingAssembly().GetName().Version;
		Title = $"CAPS - Version {version}";

		TreeViewItems =
		[
			new TreeItemViewModel("Mission 1",
			[
				new("Settings", null, OnMissionSelected),
				new("Laser Codes", null, OnMissionSelected),
				new("Notebook", null, OnMissionSelected),
				new("Prepoints", null, OnMissionSelected),
				new("Radios", null, OnTreeItemRadioListSelected),
				new("Routes", null, OnMissionSelected),
				new("TargetPoints", null, OnMissionSelected),
				new("Waypoints", null, OnTreeItemWaypointListSelected),
			], OnMissionSelected)
			{
				IsExpanded = true
			},
			//new TreeItemViewModel("Mission 2",
			//[
			//	new("Settings", null, OnMissionSelected),
			//	new("Laser Codes", null, OnMissionSelected),
			//	new("Notebook", null, OnMissionSelected),
			//	new("Prepoints", null, OnMissionSelected),
			//	new("Radios", null, OnTreeItemRadioListSelected),
			//	new("Routes", null, OnMissionSelected),
			//	new("TargetPoints", null, OnMissionSelected),
			//	new("Waypoints", null, OnTreeItemWaypointListSelected),
			//], OnMissionSelected)
		];

		// Navigate to the first item
		NavigateToWaypointList();
	}

	#region Event Handlers

	partial void OnSelectedItemChanged(TreeItemViewModel value)
	{
		value.OnSelectCommand.Execute(null);
	}

	private void OnMissionSelected(TreeItemViewModel? model)
	{
		if (model is TreeItemViewModel item)
			SelectedItem = item;
	}

	#endregion

	#region Commands

	//public RelayCommand<TreeItemViewModel> OnMissionSelectedCommand => new RelayCommand<TreeItemViewModel>(OnMissionSelected);

	[RelayCommand]
	public async Task ImportMission()
	{
		await missionManager.LoadMissionFile();
	}

	[RelayCommand]
	public async Task ExportMission()
	{
		await missionManager.ExportActiveMission();
	}

	#endregion

	#region Navigation

	[RelayCommand]
	public void NavigateToWaypointList()
	{
		NavigationService.NavigateTo<WaypointListViewModel>();
	}

	private void OnTreeItemWaypointListSelected(TreeItemViewModel? model)
	{
		OnMissionSelected(model);
		NavigationService.NavigateTo<WaypointListViewModel>();
	}

	[RelayCommand]
	public void NavigateToRadioList()
	{
		NavigationService.NavigateTo<RadioListViewModel>();
	}

	private void OnTreeItemRadioListSelected(TreeItemViewModel? model)
	{
		OnMissionSelected(model);
		NavigationService.NavigateTo<RadioListViewModel>();
	}

	#endregion
}
