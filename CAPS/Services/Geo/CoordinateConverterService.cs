﻿using CAPS.Models.Geo;
using CAPS.Services.Mission;
using CoordinateSharp;
using System.Text.RegularExpressions;

namespace CAPS.Services.Geo;

public interface ICoordinateConverterService
{
	public string LatLonToMGRS(string coordinate);
	public (double Northing, double Easting, int Zone) ToDcsCoordiantes(string coordinate);
	public (double latitude, double longitude) ParseCoordinate(string coordinate);
}

public partial class CoordinateConverterService : ICoordinateConverterService
{
	#region Constructor and Fields

	private ITheaterService _theaterService;

	public CoordinateConverterService(ITheaterService theaterService)
	{
		_theaterService = theaterService;
	}

	#endregion

	#region Generated RegEx

	/// <summary>
	/// N67:16.016 E014:21.695
	/// </summary>
	/// <returns></returns>
	[GeneratedRegex(@"([NS])\s*(\d+):(\d+):(\d+(?:\.\d+)?)\s*([EW])\s*(\d+):(\d+):(\d+(?:\.\d+)?)")]
	public static partial Regex DmsRegex();

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	[GeneratedRegex(@"([NS])\s*(\d+):(\d+(?:\.\d+)?)\s*([EW])\s*(\d+):(\d+(?:\.\d+)?)")]
	public static partial Regex DdmRegex();

	/// <summary>
	/// N 67 16.016 E 014 21.695
	/// </summary>
	/// <returns></returns>
	[GeneratedRegex(@"([NS])\s*(\d+)\s+(\d+(?:\.\d+)?)\s*([EW])\s*(\d+)\s+(\d+(?:\.\d+)?)")]
	public static partial Regex DdmSpaceSeperatedRegex();

	/// <summary>
	/// N 38° 53.000' W 77° 00.000'
	/// </summary>
	/// <returns></returns>
	[GeneratedRegex(@"([NS])\s*(\d+)°\s*(\d+(?:\.\d+)?)'\s*([EW])\s*(\d+)°\s*(\d+(?:\.\d+)?)'")]
	public static partial Regex DdmDegreeSymbolRegex();


	[GeneratedRegex(@"(\d+):(\d+):(\d+(?:\.\d+)?)")]
	private static partial Regex PartialDmsRegex();

	[GeneratedRegex(@"(\d+):(\d+(?:\.\d+)?)")]
	private static partial Regex PartialDdmRegex();

	[GeneratedRegex(@"(\d+)\s+(\d+(?:\.\d+)?)")]
	private static partial Regex PartialDdmAlternativeRegex();

	#endregion

	#region Implementation

	/// <summary>
	/// Converts a coordinate string formatted as "N67:15:10 E014:55:55" to MGRS
	/// </summary>
	/// <param name="coordinate"></param>
	/// <returns></returns>
	public string LatLonToMGRS(string coordinate)
	{
		var (latitude, longitude) = ParseCoordinate(coordinate);
		Coordinate c = new(latitude, longitude);
		string mgrs = c.MGRS.ToString();
		return mgrs;
	}

	/// <summary>
	/// Converts a coordinate string formatted as "N67:15:10 E014:55:55" to DCS coordinates
	/// </summary>
	/// <param name="coordinate"></param>
	/// <returns></returns>
	public (double Northing, double Easting, int Zone) ToDcsCoordiantes(string coordinate)
	{
		var (latitude, longitude) = ParseCoordinate(coordinate);

		var activeTheater = _theaterService.ActiveTheater;

		return UtmConverter.FromLatLon(latitude, longitude, activeTheater.UTM_zone, activeTheater.false_easting, activeTheater.false_northing);
	}

	/// <summary>
	/// Parses a coordinate string formatted as "N67:15:10 E014:55:55"
	/// </summary>
	/// <param name="coordinate"></param>
	/// <returns>A tuple of latitude and logitude as doubles</returns>
	/// <exception cref="ArgumentException"></exception>
	public (double latitude, double longitude) ParseCoordinate(string coordinate)
	{
		Match match;
		if ((match = DmsRegex().Match(coordinate)).Success)
		{
			string latitudePart = $"{match.Groups[1].Value}{match.Groups[2].Value}:{match.Groups[3].Value}:{match.Groups[4].Value}";
			string longitudePart = $"{match.Groups[5].Value}{match.Groups[6].Value}:{match.Groups[7].Value}:{match.Groups[8].Value}";

			double latitude = ParseDMS(latitudePart);
			double longitude = ParseDMS(longitudePart);

			return (latitude, longitude);
		}
		else if ((match = DdmRegex().Match(coordinate)).Success)
		{
			string latitudePart = $"{match.Groups[1].Value}{match.Groups[2].Value}:{match.Groups[3].Value}";
			string longitudePart = $"{match.Groups[4].Value}{match.Groups[5].Value}:{match.Groups[6].Value}";

			double latitude = ParseDMS(latitudePart);
			double longitude = ParseDMS(longitudePart);

			return (latitude, longitude);
		}
		else if ((match = DdmSpaceSeperatedRegex().Match(coordinate)).Success)
		{
			string latitudePart = $"{match.Groups[1].Value}{match.Groups[2].Value} {match.Groups[3].Value}";
			string longitudePart = $"{match.Groups[4].Value}{match.Groups[5].Value} {match.Groups[6].Value}";

			double latitude = ParseDMS(latitudePart);
			double longitude = ParseDMS(longitudePart);

			return (latitude, longitude);
		}
		else if ((match = DdmDegreeSymbolRegex().Match(coordinate)).Success)
		{
			string latitudePart = $"{match.Groups[1].Value}{match.Groups[2].Value}° {match.Groups[3].Value}'";
			string longitudePart = $"{match.Groups[4].Value}{match.Groups[5].Value}° {match.Groups[6].Value}'";

			double latitude = ParseDMS(latitudePart);
			double longitude = ParseDMS(longitudePart);

			return (latitude, longitude);
		}
		else
		{
			throw new ArgumentException("Invalid coordinate format");
		}
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Handles parsing of DMS coordinate parts
	/// </summary>
	/// <param name="dms"></param>
	/// <returns>Decimal Degrees as a Double</returns>
	/// <exception cref="ArgumentException"></exception>
	private static double ParseDMS(string dms)
	{
		char direction = dms[0];
		string coordinatePart = dms.Substring(1).Trim();
		double decimalDegrees;

		if (PartialDmsRegex().IsMatch(coordinatePart))
		{
			// DMS format
			var match = PartialDmsRegex().Match(coordinatePart);
			double degrees = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
			double minutes = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
			double seconds = double.Parse(match.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);

			decimalDegrees = degrees + (minutes / 60) + (seconds / 3600);
		}
		else if (PartialDdmRegex().IsMatch(coordinatePart))
		{
			// DDM format
			var match = PartialDdmRegex().Match(coordinatePart);
			double degrees = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
			double decimalMinutes = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

			decimalDegrees = degrees + (decimalMinutes / 60);
		}
		else if (PartialDdmAlternativeRegex().IsMatch(coordinatePart))
		{
			// Alternative DDM format
			var match = PartialDdmAlternativeRegex().Match(coordinatePart);
			double degrees = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
			double decimalMinutes = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

			decimalDegrees = degrees + (decimalMinutes / 60);
		}
		else
		{
			throw new ArgumentException("Invalid coordinate format");
		}

		if (direction == 'S' || direction == 'W')
			decimalDegrees = -decimalDegrees;

		return decimalDegrees;
	}

	#endregion
}
