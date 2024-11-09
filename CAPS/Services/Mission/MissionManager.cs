using CAPS.Models.Files;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;

namespace CAPS.Services.Mission;

public interface IMissionManager
{
	public List<MissionFile> MissionFiles { get; set; }
	public MissionFile ActiveMission { get; set; }
	public event EventHandler<MissionFile>? ActiveMissionChanged;
	public void UpdateActiveMissionWaypoints(ObservableCollection<Waypoint> waypoints);
	public Task LoadMissionFile();
	public Task ExportActiveMission();
}

public partial class MissionManager: ObservableObject, IMissionManager, INotifyPropertyChanged
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

		if (missionFile != null)
		{
			ActiveMission = missionFile;
			// Add the loaded mission file to the list if needed
			MissionFiles.Add(missionFile);
		}
	}

	public async Task ExportActiveMission()
	{

		var cancellationTokenSource = new CancellationTokenSource();

		await MissionFileExporter.ExportMissionFileAsync(ActiveMission, cancellationTokenSource.Token);
	}

	#endregion

}
