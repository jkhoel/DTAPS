using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Planner.Core.Class;
using Planner.Framework.Manager;
using Library.Models.Dcs.Modules.Oh58;
using Planner.Core.Service;

namespace Planner.Framework.ViewModel.Waypoint;

public partial class WaypointListItemViewModel(string name, string coordinateString, string mgrsCoordinates, double? elevation, string description, double? latitude = 0, double longitude = 0, double northing = 0, double easting = 0) : ObservableObject
{
	[ObservableProperty]
	public string name = name;

	[ObservableProperty]
	public string coordinateString = coordinateString;

	[ObservableProperty]
	public string mgrsCoordinates = mgrsCoordinates;

	[ObservableProperty]
	public double? elevation = elevation; // DCS Y

	[ObservableProperty]
	public string description = description;

	[ObservableProperty]
	public double? latitude = latitude;

	[ObservableProperty]
	public double? longitude = longitude;

	[ObservableProperty]
	public double? northing = northing; // DCS X

	[ObservableProperty]
	public double? easting = easting;  // DCS Z
}

public partial class WaypointListViewModel : ViewModelBase
{
	#region Fields and Ctor

	private readonly IMissionManager _missionManager;
	private readonly ILogger<WaypointListViewModel> _logger;
	private readonly ICoordinateSystemService _coordinateSystemService;

	private ObservableCollection<WaypointListItemViewModel> waypointItems = [];

	public WaypointListViewModel(ILogger<WaypointListViewModel> logger, ICoordinateSystemService coordinateSystemService, IMissionManager missionManager)
	{
		_logger = logger;
		_coordinateSystemService = coordinateSystemService;
		_missionManager = missionManager;

		// Subscribe to the PropertyChanged event of _missionManager
		_missionManager.ActiveMissionChanged += OnActiveMissionChanged;

		InitializeWaypoints();
	}

	private void InitializeWaypoints()
	{
		// Initialize WaypointItems
		ObservableCollection<WaypointListItemViewModel> activeWaypointItems = [];

		foreach (var wp in _missionManager.ActiveMission.Waypoints)
		{
			var dd = _coordinateSystemService.FromDcsCoordinates(wp.XCoord, wp.ZCoord);

			string coords = _coordinateSystemService.DdToDdm(dd.Latitude, dd.Longitude);
			string mgrs = _coordinateSystemService.LatLonToMGRS(coords);

			activeWaypointItems.Add(new(wp.Name, coords, mgrs, wp.YCoord, string.Empty, dd.Latitude, dd.Longitude, wp.XCoord, wp.ZCoord));
		}

		WaypointItems = activeWaypointItems;
	}


	#endregion

	#region Properties

	public ObservableCollection<WaypointListItemViewModel> WaypointItems
	{
		get => waypointItems;
		set
		{
			// Remove all listeners from the old collection
			if (waypointItems != null)
			{
				waypointItems.CollectionChanged -= OnWaypointItemsCollectionChanged;
				foreach (var item in waypointItems)
				{
					item.PropertyChanged -= OnWaypointItemPropertyChanged;
				}
			}

			SetProperty(ref waypointItems, value);

			// Add listeners to the new collection
			if (waypointItems != null)
			{
				waypointItems.CollectionChanged += OnWaypointItemsCollectionChanged;
				foreach (var item in waypointItems)
				{
					item.PropertyChanged += OnWaypointItemPropertyChanged;
				}
			}

			OnPropertyChanged(nameof(WaypointItems));
		}
	}

	#endregion

	#region Handlers

	// Handles adding or removing listeners to the individual WaypointListItemViewModel items as the list
	private void OnWaypointItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems is { } newItems)
		{
			foreach (WaypointListItemViewModel newItem in newItems)
			{
				newItem.PropertyChanged += OnWaypointItemPropertyChanged;
			}
		}

		if (e.OldItems is { } oldItems)
		{
			foreach (WaypointListItemViewModel oldItem in oldItems)
			{
				oldItem.PropertyChanged -= OnWaypointItemPropertyChanged;
			}
		}
	}

	// Handles property changes of individual WaypointListItemViewModel items
	private void OnWaypointItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender is not WaypointListItemViewModel waypointItem || e.PropertyName is not { } propertyName)
			return;

		var newValue = waypointItem.GetType().GetProperty(propertyName)?.GetValue(waypointItem);

		// Handle property changes of individual WaypointListItemViewModel items here
		System.Diagnostics.Debug.WriteLine($"Property {propertyName} of {waypointItem.Name} has changed to {newValue}");

		// Update properties
		if (propertyName == nameof(WaypointListItemViewModel.CoordinateString))
		{
			(double latitude, double longitude) = _coordinateSystemService.ParseCoordinate(waypointItem.CoordinateString);

			waypointItem.Latitude = latitude;
			waypointItem.Longitude = longitude;

			(double northing, double easting, int _) = _coordinateSystemService.ToDcsCoordiantes(waypointItem.CoordinateString);
			waypointItem.Easting = easting;
			waypointItem.Northing = northing;

			waypointItem.MgrsCoordinates = _coordinateSystemService.LatLonToMGRS(waypointItem.CoordinateString);
		}

		UpdateActiveMissionWaypoints();
	}

	// Update the ActiveMission's Waypoints collection based on the WaypointItems collection
	private void UpdateActiveMissionWaypoints()
	{

		ObservableCollection<Library.Models.Dcs.Modules.Oh58.Waypoint> newWaypoints = [];

		foreach (var item in WaypointItems)
		{
			newWaypoints.Add(new Library.Models.Dcs.Modules.Oh58.Waypoint(item.Name, item.Northing ?? 0, item.Elevation ?? 0, item.Easting ?? 0));
		}

		_missionManager.UpdateActiveMissionWaypoints(newWaypoints);
	}

	// Handles changes to the ActiveMission's Waypoints collection
	private void OnActiveMissionChanged(object? sender, MissionFile newActiveMission)
	{
		InitializeWaypoints();
	}

	#endregion

	[RelayCommand]
	public void AddWaypoint()
	{
		WaypointItems.Add(new("New Waypoint", "N 0 0.0 E 0 0.0", "32U DB 12345 12345", 0, string.Empty));
	}

	[RelayCommand]
	public void DeleteWaypoint()
	{
		if (WaypointItems.Count > 0)
			WaypointItems.RemoveAt(WaypointItems.Count - 1);
	}
}