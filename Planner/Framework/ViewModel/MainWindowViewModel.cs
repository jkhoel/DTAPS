using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library.Models.Dcs.Modules.Oh58;
using Planner.Core.Class;
using Planner.Core.Service;
using Planner.Framework.Manager;
using Planner.Framework.ViewModel.MissionSettings;
using Planner.Framework.ViewModel.PlannerTree;
using Planner.Framework.ViewModel.Radio;
using Planner.Framework.ViewModel.Waypoint;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;

namespace Planner.Framework.ViewModel;

public partial class MainWindowViewModel : ViewModelBase
{
	[ObservableProperty]
	private ObservableCollection<TreeItemViewModel> treeViewItems = new();

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

		// Initialize (build) the navigation tree
		UpdateNavigationTree();

		// Navigate to the first item
		NavigateToMissionSettings();

		missionManager.ActiveMissionChanged += OnMissionActiveMissionChanged;
		missionManager.MissionListChanged += OnMissionsListChanged;
	}

	private void UpdateNavigationTree()
	{
		var treeItems = new ObservableCollection<TreeItemViewModel>();

		var index = 0;

		foreach (var mission in MissionManager.MissionList)
		{
			var missionName = string.IsNullOrEmpty(mission.MissionName) ? $"Mission {index + 1}" : $"Mission {index + 1}: {mission.MissionName}";

			var children = new ObservableCollection<TreeItemViewModel>
			{
				new(index, "Laser Codes", null, OnMissionSelected),
				new(index, "Notebook", null, OnMissionSelected),
				new(index, "Prepoints", null, OnMissionSelected),
				new(index, "Radios", null, OnTreeItemRadioListSelected),
				new(index, "Routes", null, OnMissionSelected),
				new(index, "TargetPoints", null, OnMissionSelected),
				new(index, "Waypoints", null, OnTreeItemWaypointListSelected)
			};

			var ctxMenu = new ContextMenu();
			ctxMenu.Items.Add(new MenuItem { Header = "Delete Mission", Command = DeleteMissionCommand, CommandParameter = index });

			var missionItem = new TreeItemViewModel(index, missionName, children, OnTreeMissionSettingsSelected)
			{
				ContextMenu = ctxMenu,
				IsExpanded = true
			};

			treeItems.Add(missionItem);

			index++;
		}

		TreeViewItems = treeItems;
	}

	#region Event Handlers

	partial void OnSelectedItemChanged(TreeItemViewModel value)
	{
		value.OnSelectCommand.Execute(null);
	}

	private void OnMissionSelected(TreeItemViewModel? model)
	{
		if (model is not TreeItemViewModel item)
			return;

		// Update selected item
		SelectedItem = item;

		// Update the active mission
		MissionManager.UpdateActiveMission(MissionManager.MissionList[item.GroupIndex]);
	}

	private void OnMissionActiveMissionChanged(object? sender, MissionFile e)
	{
		//UpdateNavigationTree();
	}

	private void OnMissionsListChanged(object? sender, ObservableCollection<MissionFile> e)
	{
		UpdateNavigationTree();
	}

	#endregion

	#region Commands

	//public RelayCommand<TreeItemViewModel> OnMissionSelectedCommand => new RelayCommand<TreeItemViewModel>(OnMissionSelected);

	[RelayCommand]
	public Task DeleteMission(int index)
	{
		MissionManager.RemoveMission(index);

		return Task.CompletedTask;
	}

	[RelayCommand]
	public async Task ImportMission()
	{
		await MissionManager.LoadMissionFile();
	}

	[RelayCommand]
	public async Task ExportMission()
	{
		await MissionManager.ExportActiveMission();
	}

	#endregion

	#region Navigation

	[RelayCommand]
	public void NavigateToMissionSettings()
	{
		NavigationService.NavigateTo<MissionSettingsViewModel>();
	}

	private void OnTreeMissionSettingsSelected(TreeItemViewModel? model)
	{
		OnMissionSelected(model);
		NavigationService.NavigateTo<MissionSettingsViewModel>();
	}

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
