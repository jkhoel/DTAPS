using CommunityToolkit.Mvvm.ComponentModel;
using Library.Models.Dcs.Modules.Oh58;
using Planner.Core.Service;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Planner.Framework.Manager;

public interface IMissionManager
{
	public ObservableCollection<MissionFile> MissionList { get; set; }
	public MissionFile ActiveMission { get; set; }
	public void UpdateActiveMission(MissionFile mission);
	public void UpdateActiveMissionWaypoints(ObservableCollection<Waypoint> waypoints);
	public Task LoadMissionFile();
	public Task ExportActiveMission();

	public event EventHandler<MissionFile>? ActiveMissionChanged;

	public event EventHandler<ObservableCollection<MissionFile>>? MissionListChanged;
}

public partial class MissionManager : ObservableObject, IMissionManager //, INotifyPropertyChanged
{
	#region Properties

	[ObservableProperty]
	private ObservableCollection<MissionFile> missionList = [];

	[ObservableProperty]
	private MissionFile activeMission = new();

	#endregion

	#region Constructor

	public MissionManager()
	{
		MissionList.CollectionChanged += OnMissionListCollectionChanged;

		MissionList.Add(ActiveMission);
	}

	

	#endregion

	#region Events and Handlers

	public event EventHandler<MissionFile>? ActiveMissionChanged;
	partial void OnActiveMissionChanged(MissionFile value) => ActiveMissionChanged?.Invoke(this, value);

	public event EventHandler<ObservableCollection<MissionFile>>? MissionListChanged;

	private void OnMissionListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		//MissionListChanged?.Invoke(this, value);
		MissionListChanged?.Invoke(this, MissionList);
	}

	#endregion

	#region Methods

	public void UpdateActiveMission(MissionFile mission)
	{
		ActiveMission = mission;
	}

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
			MissionList.Add(file);
		}
	}

	public async Task ExportActiveMission()
	{
		var cancellationTokenSource = new CancellationTokenSource();
		await MissionFileExporter.ExportMissionFileAsync(ActiveMission, cancellationTokenSource.Token);
	}

	#endregion

}
