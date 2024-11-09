using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace CAPS.Models.Files;

/// <summary>
/// Describes the expected fields found in the KW's mission JSON files
/// </summary>


// TODO: Figure out if some of the list needs to be padded to an expected number
// TODO: Figure out what is the maximum number of entries into the lists
// TODO: Add the two figures above as comments for the records below

public class MissionFile
{
	[JsonPropertyName("battlefieldgraphics")]
	public List<BattlefieldGraphic> BattlefieldGraphics { get; set; } = new List<BattlefieldGraphic>();

	[JsonPropertyName("legalversion")]
	public string? LegalVersion { get; set; } = "100";

	[JsonPropertyName("missionName")]
	// Max length: 6
	public string? MissionName { get; set; } = CreateMissionName();

	[JsonPropertyName("notebook")]
	// Max length: 200?
	public List<NoteBookItem> Notebook { get; set; } = new List<NoteBookItem>();

	[JsonPropertyName("lasercodes")]
	// Max length: 8
	public List<LaserCodeItem> LaserCodes { get; set; } = new List<LaserCodeItem>();

	[JsonPropertyName("prepoints")]
	public List<PrePoint> PrePoints { get; set; } = new List<PrePoint>();

	[JsonPropertyName("radios")]
	public Radios? Radios { get; set; } = new();

	[JsonPropertyName("routes")]
	public List<Route>? Routes { get; set; }

	[JsonPropertyName("targetpoints")]
	public List<TargetPoint>? TargetPoints { get; set; }

	[JsonPropertyName("waypoints")]
	public ObservableCollection<Waypoint> Waypoints { get; set; } = new ObservableCollection<Waypoint>();

	#region Mission CRUD

	public static string CreateMissionName()
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		const string digits = "0123456789";

		return GetRandomFromChars(2, chars) + GetRandomFromChars(4, digits);
	}

	// TODO: Implement validation of file on load
	//private readonly FileModelValidator _validator = new(1024 * 500, "application/json");
	//
	//public async Task<Result> UpdateMissionFromBrowserFile(IBrowserFile? file)
	//{
	//	var result = await _validator.ValidateAsync(new FileModel(file?.Name, file));
	//
	//	if (file is null || !result.IsValid)
	//		return new(false, result.Errors.ConvertAll(e => e.ErrorMessage));
	//
	//	var stream = file.OpenReadStream();
	// .... etc.
	public async static Task<MissionFile> LoadFromFile(string path)
	{
		FileStream? stream = null;

		try
		{
			stream = new FileStream(path, FileMode.Open);

			if (stream.CanRead == false)
			{
				throw new Exception("Cannot read file");
			}

			var missionFile = await Task.FromResult(JsonSerializer.Deserialize<MissionFile>(stream));

			if (missionFile is null)
			{
				throw new Exception("Failed to load mission file");
			}

			return missionFile;
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to load mission file", ex);
		}
		finally
		{
			stream?.Close();
		}
	}

	#endregion

	#region Private Helpers

	private static Random random = new Random();

	private static string GetRandomFromChars(int length, string chars)
	{
		return new string(
			Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
		);
	}
	#endregion
}

public record BattlefieldGraphic(string Name, int? Type = 2)
{
	[JsonPropertyName("points")]
	public List<BattlefieldGraphicPoint> Points { get; set; } = new List<BattlefieldGraphicPoint>();

	[JsonPropertyName("type")]
	public int? Type { get; set; } = Type;
}

public record BattlefieldGraphicPoint(int Index = 0, int Type = 0)
{
	[JsonPropertyName("index")]
	public int Index { get; set; } = Index;

	[JsonPropertyName("type")]
	public int Type { get; set; } = Type;
};

/// <summary>
/// A note book entry of maximum 37 characters
/// </summary>
/// <param name="Text"></param>
public record NoteBookItem(string Text)
{
	[JsonPropertyName("text")]
	public string Text { get; set; } = Text;
}

/// <summary>
/// A Laser code of maximum 4 digits
/// </summary>
/// <param name="Code"></param>
public record LaserCodeItem(int Code)
{
	[JsonPropertyName("code")]
	public int Code { get; set; } = Code;
}

public record PrePoint(int Index = -1, int Type = -1)
{
	[JsonPropertyName("index")]
	public int Index { get; set; } = Index;

	[JsonPropertyName("type")]
	public int Type { get; set; } = Type;
}

public record Radios
{
	[JsonPropertyName("uam")]
	public List<RadioPreset>? Uam { get; set; } = new List<RadioPreset>();

	[JsonPropertyName("vam")]
	public List<RadioPreset>? Vam { get; set; } = new List<RadioPreset>();

	[JsonPropertyName("vfm1")]
	public List<RadioPreset>? Vfm1 { get; set; } = new List<RadioPreset>();

	[JsonPropertyName("vfm2")]
	public List<RadioPreset>? Vfm2 { get; set; } = new List<RadioPreset>();
}

public record RadioPreset(string Name, double Frequency)
{
	private const int Multiplier = 1000000;

	[JsonPropertyName("name")]
	public string Name { get; set; } = Name;

	[JsonPropertyName("freq")]
	public double Frequency { get; set; } = Frequency;

	public string ToFormat(string format)
	{
		return string.Format(format, GetFrequency());
	}

	public double GetFrequency(int divisor = Multiplier)
	{
		return Frequency / divisor;
	}

	public void SetFrequency(double frequency, int multiplier = Multiplier)
	{
		Frequency = frequency * multiplier;
	}
}

public record Route(string Name)
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = Name;

	[JsonPropertyName("points")]
	public List<RoutePoint> Points { get; set; } = new List<RoutePoint>();
}

public record RoutePoint(int Index, int Type = 0)
{
	[JsonPropertyName("index")]
	public int Index { get; set; } = Index;

	[JsonPropertyName("type")]
	public int Type { get; set; } = Type;
}

public record TargetPoint(string Name, double XCoord, double YCoord, double ZCoord)
{
	[JsonPropertyName("artynumber")]
	public string? ArtyNumber { get; set; }

	[JsonPropertyName("date")]
	public string? Date { get; set; }

	[JsonPropertyName("datum")]
	public int Datum { get; set; } = 47;

	[JsonPropertyName("firemission")]
	public string? FireMission { get; set; }

	[JsonPropertyName("firemissionstatus")]
	public string? FireMissionStatus { get; set; }

	[JsonPropertyName("fom")]
	public int Fom { get; set; } = 1;

	[JsonPropertyName("name")]
	public string Name { get; set; } = Name;

	[JsonPropertyName("quantity")]
	public int Quantity { get; set; } = 0;

	[JsonPropertyName("subtype")]
	public int SubType { get; set; } = 108;

	[JsonPropertyName("time")]
	public string? Time { get; set; }

	[JsonPropertyName("x")]
	public double XCoord { get; set; } = XCoord;

	[JsonPropertyName("y")]
	public double YCoord { get; set; } = YCoord;

	[JsonPropertyName("z")]
	public double ZCoord { get; set; } = ZCoord;
}

public record Waypoint(string Name, double XCoord, double YCoord, double ZCoord)
{
	[JsonPropertyName("datum")]
	public int Datum { get; set; } = 47;

	[JsonPropertyName("name")]
	public string Name { get; set; } = Name;

	[JsonPropertyName("x")]
	public double XCoord { get; set; } = XCoord;

	[JsonPropertyName("y")]
	public double YCoord { get; set; } = YCoord;

	[JsonPropertyName("z")]
	public double ZCoord { get; set; } = ZCoord;
}


