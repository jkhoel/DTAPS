namespace CAPS.Models.Geo;

public class Theater
{
	public string? Name { get; set; }
	public double false_northing { get; set; }
	public double false_easting { get; set; }
	public int UTM_zone { get; set; }
	public string? Hemisphere { get; set; }
	public int WinterTimeDelta { get; set; }
	public int SummerTimeDelta { get; set; }
}
