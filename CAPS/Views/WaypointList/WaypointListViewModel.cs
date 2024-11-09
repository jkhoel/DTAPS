using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace CAPS.Views.WaypointList;

public class WaypointListItem(string name, string coordinateString, string mgrsCoordinates, string elevation)
{
	public string Name { get; set; } = name;
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public string CoordinateString { get; set; } = coordinateString;
	public string MgrsCoordinates { get; set; } = mgrsCoordinates;
	public string Elevation { get; set; } = elevation;
}

internal partial class WaypointListViewModel : ObservableObject
{
	private ObservableCollection<WaypointListItem> waypointItems = [];

	public ObservableCollection<WaypointListItem> WaypointItems
	{
		get => waypointItems;
		set
		{
			SetProperty(ref waypointItems, value);
			OnPropertyChanged(nameof(WaypointItems));
		}
	}

	public WaypointListViewModel()
	{
		WaypointItems = [
			new WaypointListItem("W1", "N 67 16.016 E 014 21.695", "18S UJ 228 308", "1000"),
			new WaypointListItem("W2", "N 38° 53.000' W 77° 00.000'", "18S UJ 228 308", "200"),
			];
	}
}
