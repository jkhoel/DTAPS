using CAPS.Models.Files;
using CAPS.Views.WaypointList;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAPS.Services.Mission;

public interface IMissionManager
{
	public List<MissionFile> MissionFiles { get; set; }
	public MissionFile ActiveMission { get; set; }
}

public partial class MissionManager: ObservableObject, IMissionManager
{
	public List<MissionFile> MissionFiles { get; set; } = [];

	[ObservableProperty]
	private MissionFile activeMission = new();

	public MissionManager()
    {
		MissionFiles.Add(ActiveMission);

		ActiveMission.Waypoints =
		[
			new("W1", -66957.796875, 20, -348352.28125),
			new("W2", -64776.8203125, 0, -351449.53125),
			new("W3", -67193.35774689168, 0, -348526.5665485276),
			new("W4", 0, 0, 0),
			new("W5", 0, 0, 0),
			new("W6", 0, 0, 0),
			new("W7", 0, 0, 0),
			new("W8", 0, 0, 0),
			new("W9", 0, 0, 0),
			new("W10", 0, 0, 0),
			new("W11", 0, 0, 0),
			new("W12", 0, 0, 0),
			new("W13", 0, 0, 0),
			new("W14", 0, 0, 0),
			new("W15", 0, 0, 0),
			new("W16", 0, 0, 0),
			new("W17", 0, 0, 0),
			new("W18", 0, 0, 0),
			new("W19", 0, 0, 0),
			new("W20", 0, 0, 0),
			new("W21", 0, 0, 0),
			new("W22", 0, 0, 0),
			new("W23", 0, 0, 0),
			new("W24", 0, 0, 0),
			new("W25", 0, 0, 0),
			new("W26", 0, 0, 0),
			new("W27", 0, 0, 0),
			new("W28", 0, 0, 0),
			new("W29", 0, 0, 0),
			new("W30", 0, 0, 0),
		];
	}

	public void UpdateActiveMissionWaypoints(ObservableCollection<Waypoint> waypoints)
	{
		ActiveMission.Waypoints = waypoints;
	}
}
