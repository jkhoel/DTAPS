using CAPS.Models.Geo;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAPS.Services.Mission;

public interface ITheaterService
{
	public Theater ActiveTheater { get; set; }
	public static Theaters Theaters => new();
	public void SetActiveTheater(Theater theater);
}

public partial class TheaterService : ObservableObject, ITheaterService
{
	#region Properties

	[ObservableProperty]
	private Theater activeTheater;

	public static Theaters Theaters => new();

	#endregion

	#region Fields and Constructors

	public TheaterService()
	{
		ActiveTheater = Theaters.Kola;
	}

	#endregion

	#region Implementation

	public void SetActiveTheater(Theater theater)
	{
		ActiveTheater = theater;
	}

	#endregion
}

public class Theaters
{
	public Theater Kola = new()
	{
		Name = "Kola",
		false_northing = -7543624.999999979,
		false_easting = -62702.00000000087,
		UTM_zone = 34,
		Hemisphere = "N",
		WinterTimeDelta = 1,
		SummerTimeDelta = 2
	};

	// TODO: Values are not verified
	public Theater Syria = new()
	{
		Name = "Syria",
		false_northing = 3879865,
		false_easting = 217198,
		UTM_zone = 37,
		Hemisphere = "N",
		WinterTimeDelta = 3,
		SummerTimeDelta = 4
	};
}

