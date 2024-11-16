using CommunityToolkit.Mvvm.ComponentModel;
using Library.Models.Dcs.Modules.Oh58;
using Microsoft.Extensions.Logging;
using Planner.Core.Class;
using Planner.Core.Service;
using Planner.Framework.Manager;
using Planner.Framework.ViewModel.Waypoint;
using System;
using System.Collections.Generic;
namespace Planner.Framework.ViewModel.MissionSettings;

public partial class MissionSettingsViewModel: ViewModelBase
{
	private readonly IMissionManager _missionManager;
	private readonly ILogger<MissionSettingsViewModel> _logger;
	private readonly INavigationService _navigationService;

	[ObservableProperty]
	private string missionName = "New Mission";

	public MissionSettingsViewModel(ILogger<MissionSettingsViewModel> logger, IMissionManager missionManager, INavigationService navigationService)
	{
		_logger = logger;
		_missionManager = missionManager;
		_navigationService = navigationService;

		// Subscribe to the PropertyChanged event of _missionManager
		_missionManager.ActiveMissionChanged += OnActiveMissionChanged;

		InitializeMissionSettings();
	}

	private void InitializeMissionSettings()
	{
		UpdateMissionSettingsFromActiveMission();
	}

	private void OnActiveMissionChanged(object? sender, MissionFile e)
	{
		UpdateMissionSettingsFromActiveMission();
	}

	private void UpdateMissionSettingsFromActiveMission()
	{
		MissionName = _missionManager.ActiveMission.MissionName ?? MissionName;
	}
}
