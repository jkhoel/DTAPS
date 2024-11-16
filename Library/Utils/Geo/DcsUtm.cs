namespace Library.Utils.Geo;

/// <summary>
/// Creates a new instance of the UTM converter based on the WSG84 ellipsoid and the provided parameters
/// </summary>

// Resources:
// https://wiki.openstreetmap.org/wiki/Mercator#C#_implementation
// https://github.com/pydcs/dcs/blob/master/dcs/terrain/projections/transversemercator.py
// https://epsg.io/9807-method
// https://proj.org/en/9.4/operations/projections/tmerc.html

public class DcsUtm
{
    // Semi-major axis of the WGS84 ellipsoid
    private const double A = 6378137;

    // Flattening of the WGS84 ellipsoid
    private const double F = 1 / 298.257223563;

    // Scale factor for UTM
    private const double K0 = 0.9996;

    // Eccentricity squared
    private const double EccSquared = F * (2 - F);

    // Secondary eccentricity squared
    private const double EccPrimeSquared = EccSquared / (1 - EccSquared);

    /// <summary>
    /// Converts DCS UTM coordinates to Latitude and Longitude in Decimal Degrees
    /// </summary>
    /// <param name="easting"></param>
    /// <param name="northing"></param>
    /// <param name="zone"></param>
    /// <param name="isSouthernHemisphere"></param>
    /// <param name="falseEasting"></param>
    /// <param name="falseNorthing"></param>
    /// <returns></returns>
    public static (double Latitude, double Longitude) ToLatLon(
        double easting,
        double northing,
        int zone = 31,
        bool isSouthernHemisphere = false,
        double falseEasting = 500000,
        double falseNorthing = 0
    )
    {
        // Correct for false easting and northing
        easting -= falseEasting;
        northing -= falseNorthing;

        if (isSouthernHemisphere)
        {
            northing += 10000000; // Correct for southern hemisphere
        }

        // Calculate the meridional arc
        var m = northing / K0;

        // Calculate footprint latitude
        var mu =
            m
            / (
                A
                * (
                    1
                    - EccSquared / 4
                    - 3 * EccSquared * EccSquared / 64
                    - 5 * EccSquared * EccSquared * EccSquared / 256
                )
            );

        // Calculate latitude in radians
        var e1 = (1 - Math.Sqrt(1 - EccSquared)) / (1 + Math.Sqrt(1 - EccSquared));
        var phi1Rad =
            mu
            + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * mu)
            + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * mu)
            + 151 * e1 * e1 * e1 / 96 * Math.Sin(6 * mu);

        // Calculate longitude offset in radians
        var c1 = EccSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad) / (1 - EccSquared);
        var t1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
        var n1 = A / Math.Sqrt(1 - EccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad));
        var r1 =
            A
            * (1 - EccSquared)
            / Math.Pow(1 - EccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad), 1.5);
        var d = easting / (n1 * K0);

        // Calculate latitude
        var latRad =
            phi1Rad
            - n1 * Math.Tan(phi1Rad) / r1
                * (
                    d * d / 2
                    - (
                        5
                        + 3 * t1
                        + 10 * c1
                        - 4 * c1 * c1
                        - 9 * EccSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad)
                    )
                        * Math.Pow(d, 4)
                        / 24
                    + (
                        61
                        + 90 * t1
                        + 298 * c1
                        + 45 * t1 * t1
                        - 252 * EccSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad)
                    )
                        * Math.Pow(d, 6)
                        / 720
                );

        // Calculate the Central Meridian of the Zone
        var lonOrigin = CalculateCentralMeridian(zone, easting);

        // Calculate longitude
        var lonRad =
            (
                d
                - (1 + 2 * t1 + c1) * Math.Pow(d, 3) / 6
                + (
                    5
                    - 2 * c1
                    + 28 * t1
                    - 3 * c1 * c1
                    + 8 * EccSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad)
                    + 24 * t1 * t1
                )
                    * Math.Pow(d, 5)
                    / 120
            ) / Math.Cos(phi1Rad);
        lonRad += DegToRad(lonOrigin); // Adjust with central meridian

        // Convert radians to degrees
        var latitude = RadToDeg(latRad);
        var longitude = RadToDeg(lonRad);

        return (latitude, longitude);
    }

    /// <summary>
    /// Converts Latitude and Logitude in Decimal Degrees to DCS UTM coordinates
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    public static (double Northing, double Easting, int Zone) FromLatLon(
        double latitude,
        double longitude,
        int? zone = null,
        double falseEasting = 500000.0,
        double falseNorthing = 0.0
)
    {
        // Make sure zone overrides are within valid range
        var zoneNumber = zone switch
        {
            >= 1 and <= 60 => zone.Value,
            _ => CalculateZoneNumber(latitude, longitude)
        };

        var latRad = DegToRad(latitude);
        var longRad = DegToRad(longitude);
        var longOriginRad = CalculateLongOrigin(zoneNumber);

        var n = A / Math.Sqrt(1 - EccSquared * Math.Sin(latRad) * Math.Sin(latRad));
        var t = Math.Tan(latRad) * Math.Tan(latRad);
        var c = EccPrimeSquared * Math.Cos(latRad) * Math.Cos(latRad);
        var a = Math.Cos(latRad) * (longRad - longOriginRad);

        var m = CalculateM(latRad);

        var foo = CalculateEasting(a, n, t, c);

        var utmEasting = CalculateEasting(a, n, t, c) + falseEasting;
        var utmNorthing = CalculateNorthing(m, n, t, c, a, latRad) + falseNorthing;
        if (latitude < 0)
        {
            utmNorthing += 10000000.0; // Offset for southern hemisphere
        }

        return (utmNorthing, utmEasting, zoneNumber);
    }

    #region Calulations

    private static int CalculateZoneNumber(double latitude, double longitude)
    {
        var zoneNumber = (int)((longitude + 180) / 6) + 1;

        zoneNumber = latitude switch
        {
            // Special zones for Norway/Svalbard
            >= 56.0 and < 64.0 when longitude is >= 3.0 and < 12.0 => 32,
            >= 72.0
            and < 84.0
                => longitude switch
                {
                    >= 0.0 and < 9.0 => 31,
                    >= 9.0 and < 21.0 => 33,
                    >= 21.0 and < 33.0 => 35,
                    >= 33.0 and < 42.0 => 37,
                    _ => zoneNumber
                },
            _ => zoneNumber
        };
        return zoneNumber;
    }

    /// <summary>
    /// Calculate the Central Meridian of the Zone, taking into account adjusted central meridian for special zones (Svalbard)
    /// </summary>
    /// <param name="zone"></param>
    /// <param name="easting"></param>
    /// <returns></returns>
    private static double CalculateCentralMeridian(int zone, double easting)
    {
        // Simplified logic for adjusting longitude origin for special zones
        var z = zone - 1;

        return zone switch
        {
            31 => easting >= 500000 ? 3 : z * 6 - 180 + 3,
            33 => easting >= 500000 ? 9 : z * 6 - 180 + 3,
            35 => easting >= 500000 ? 21 : z * 6 - 180 + 3,
            37 => easting >= 500000 ? 33 : z * 6 - 180 + 3,
            _ => z * 6 - 180 + 3
        };
    }

    private static double CalculateLongOrigin(int zoneNumber)
    {
        return DegToRad((zoneNumber - 1) * 6 - 180 + 3);
    }

    private static double CalculateM(double latRad)
    {
        return A
            * (
                (
                    1
                    - EccSquared / 4
                    - 3 * EccSquared * EccSquared / 64
                    - 5 * EccSquared * EccSquared * EccSquared / 256
                ) * latRad
                - (
                    3 * EccSquared / 8
                    + 3 * EccSquared * EccSquared / 32
                    + 45 * EccSquared * EccSquared * EccSquared / 1024
                ) * Math.Sin(2 * latRad)
                + (
                    15 * EccSquared * EccSquared / 256
                    + 45 * EccSquared * EccSquared * EccSquared / 1024
                ) * Math.Sin(4 * latRad)
                - 35 * EccSquared * EccSquared * EccSquared / 3072 * Math.Sin(6 * latRad)
            );
    }

    private static double CalculateEasting(double A, double N, double T, double C)
    {
        return K0
            * N
            * (
                A
                + (1 - T + C) * Math.Pow(A, 3) / 6
                + (5 - 18 * T + T * T + 72 * C - 58 * EccPrimeSquared) * Math.Pow(A, 5) / 120
            );
    }

    private static double CalculateNorthing(
        double m,
        double n,
        double t,
        double c,
        double a,
        double latRad
    )
    {
        return K0
            * (
                m
                + n
                    * Math.Tan(latRad)
                    * (
                        Math.Pow(a, 2) / 2
                        + (5 - t + 9 * c + 4 * c * c) * Math.Pow(a, 4) / 24
                        + (61 - 58 * t + t * t + 600 * c - 330 * EccPrimeSquared)
                            * Math.Pow(a, 6)
                            / 720
                    )
            );
    }

    #endregion

    #region Conversions

    private static double DegToRad(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static double RadToDeg(double radians)
    {
        return radians * (180.0 / Math.PI);
    }

    #endregion
}
