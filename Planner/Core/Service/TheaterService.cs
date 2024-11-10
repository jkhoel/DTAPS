using CommunityToolkit.Mvvm.ComponentModel;
using Library.Models.Dcs;

namespace Planner.Core.Service;

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