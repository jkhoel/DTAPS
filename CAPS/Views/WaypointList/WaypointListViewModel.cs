﻿using CAPS.Services;
using CAPS.Services.Geo;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;


namespace CAPS.Views.WaypointList;

public partial class WaypointListItemViewModel(string name, string coordinateString, string mgrsCoordinates, string elevation) : ObservableObject
{
	[ObservableProperty]
	public string name = name;

	[ObservableProperty]
	public string coordinateString = coordinateString;

	[ObservableProperty]
	public string mgrsCoordinates = mgrsCoordinates;

	[ObservableProperty]
	public string elevation = elevation;

	[ObservableProperty]
	public double latitude;

	[ObservableProperty]
	public double longitude;

	[ObservableProperty]
	public double northing; // DCS X

	[ObservableProperty]
	public double easting;	// DCS Z
}

public partial class WaypointListViewModel : ObservableObject
{
	#region Fields and Ctor

	private readonly ILogger<WaypointListViewModel> _logger;

	private readonly ICoordinateConverterService _coordinateConverter;

	private ObservableCollection<WaypointListItemViewModel> waypointItems = [];

	public WaypointListViewModel(ILogger<WaypointListViewModel> logger, ICoordinateConverterService coordinateConverter)
	{
		_logger = logger;
		_coordinateConverter = coordinateConverter;


		WaypointItems = [
			new WaypointListItemViewModel("W1", "N 67 16.016 E 014 21.695", "33W VQ 72469 61279", "1000"),
			new WaypointListItemViewModel("W2", "N67:16:9.07 E014:21:54.16", "33W VQ 72621 61528", "200"),
			//new WaypointListItemViewModel("W3", "N 38° 53.000' W 77° 00.000'", "18S UJ 22800 30800", "200"), // Format not supported yet
			];
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
			(double latitude, double longitude) = _coordinateConverter.ParseCoordinate(waypointItem.CoordinateString);

			waypointItem.Latitude = latitude;
			waypointItem.Longitude = longitude;

			(double northing, double easting, int _) = _coordinateConverter.ToDcsCoordiantes(waypointItem.CoordinateString);
			waypointItem.Northing = northing;
			waypointItem.Easting = easting;

			waypointItem.MgrsCoordinates = _coordinateConverter.LatLonToMGRS(waypointItem.CoordinateString);
		}
	}

	#endregion
}
