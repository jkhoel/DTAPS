using CommunityToolkit.Mvvm.ComponentModel;
using Library.Models.Dcs.Modules.Oh58;
using Planner.Core.Service;
using System.Collections.ObjectModel;

namespace Planner.Framework.Manager;

public interface IMissionManager
{
	public List<MissionFile> MissionFiles { get; set; }
	public MissionFile ActiveMission { get; set; }
	public void UpdateActiveMissionWaypoints(ObservableCollection<Waypoint> waypoints);
	public Task LoadMissionFile();
	public Task ExportActiveMission();

	public event EventHandler<MissionFile>? ActiveMissionChanged;
}

public partial class MissionManager : ObservableObject, IMissionManager //, INotifyPropertyChanged
{
	#region Properties

	public List<MissionFile> MissionFiles { get; set; } = [];

	[ObservableProperty]
	private MissionFile activeMission = new();

	#endregion

	#region Constructor

	public MissionManager()
	{
		MissionFiles.Add(ActiveMission);
	}

	#endregion

	#region Events and Handlers

	public event EventHandler<MissionFile>? ActiveMissionChanged;

	partial void OnActiveMissionChanged(MissionFile value) => ActiveMissionChanged?.Invoke(this, value);

	#endregion

	#region Methods

	public void UpdateActiveMissionWaypoints(ObservableCollection<Waypoint> waypoints)
	{
		ActiveMission.Waypoints = waypoints;
	}

	public async Task LoadMissionFile()
	{
		var cancellationTokenSource = new CancellationTokenSource();
		var missionFile = await MissionFileImporter.ImportMissionFileAsync(cancellationTokenSource.Token);

		if (missionFile is { } file)
		{
			ActiveMission = file;
			MissionFiles.Add(file);
		}
	}

	public async Task ExportActiveMission()
	{
		var cancellationTokenSource = new CancellationTokenSource();
		await MissionFileExporter.ExportMissionFileAsync(ActiveMission, cancellationTokenSource.Token);
	}

	#endregion

}
