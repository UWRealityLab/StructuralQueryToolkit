using MathNet.Numerics;
using UnityEngine;

public static class TwoDimensionalStereonetUtils
{
    private const float TREND_OFFSET = 90f * Mathf.Deg2Rad;
    private const float STRIKE_OFFSET = -90f * Mathf.Deg2Rad;

    /// <summary>
    /// Returns a 2D position with the given trend and plunge.
    /// </summary>
    /// <param name="stereonetRadius">The radius of the stereonet image</param>
    /// <param name="normal">Normal of the pole</param>
    public static Vector2 GetPolePosition(float stereonetRadius, Vector3 normal)
    {
        StereonetUtils.CalculateTrendAndPlunge(normal, out var trend, out var plunge);
        return GetPolePosition(stereonetRadius, trend, plunge);
    }

    /// <summary>
    /// Returns a 2D position with the given trend and plunge.
    /// </summary>
    /// <param name="stereonetRadius">The radius of the stereonet image</param>
    /// <param name="trend">Trend in degrees</param>
    /// <param name="plunge">Plunge in degrees</param>
    public static Vector2 GetPolePosition(float stereonetRadius, float trend, float plunge)
    {
        var trendRadians = -trend * Mathf.Deg2Rad + TREND_OFFSET;
        var plungeRadians = plunge * Mathf.Deg2Rad;

        var plungeRadius = GetRadiusProjection(stereonetRadius, plungeRadians);
        
        return ConvertPolarToCartesian(trendRadians, plungeRadius);
    }

    /// <summary>
    /// Returns points that represent the 2D line of a plane on a stereonet.
    /// </summary>
    /// <param name="stereonetRadius">The radius of the stereonet image</param>
    /// <param name="normal"> Normal of the pole </param>
    /// <param name="numPoints">Number of points in the curve</param>
    public static Vector2[] GetPlaneLinePoints(float stereonetRadius, Vector3 normal, int numPoints)
    {
        StereonetUtils.CalculateStrikeAndDip(normal, out var strike, out var dip);
        return GetPlaneLinePoints(stereonetRadius, strike, dip, numPoints);
    }

    /// <summary>
    /// Returns points that represent the 2D line of a plane on a stereonet.
    /// </summary>
    /// <param name="stereonetRadius">The radius of the stereonet image</param>
    /// <param name="strike">Strike in degrees</param>
    /// <param name="dip">Strike in degrees</param>
    /// <param name="numPoints">Number of points in the curve</param>
    public static Vector2[] GetPlaneLinePoints(float stereonetRadius, float strike, float dip, int numPoints)
    {
        var strikeRadians = -strike * Mathf.Deg2Rad + STRIKE_OFFSET;
        var dipRadians = dip * Mathf.Deg2Rad;

        if (dip.AlmostEqual(90f, float.Epsilon))
        {
            // Edge case: if dip is 90 degrees, return a straight line
            return new Vector2[]
            {
                ConvertPolarToCartesian(strikeRadians, stereonetRadius),
                ConvertPolarToCartesian(strikeRadians + Mathf.PI, stereonetRadius)
            };
        }        

        var curvePoints = new Vector2[numPoints];

        var strikeIncrement = Mathf.PI / (numPoints - 1);
        var tanDip = Mathf.Tan(dipRadians);
        for (int i = 0; i < curvePoints.Length; i++)
        {
            // https://run.unl.pt/bitstream/10362/4500/1/CT_11_18.pdf pg.10 (Section II.1)
            var currStrikeRad = strikeRadians + (i * strikeIncrement);
            var currDipRad = Mathf.Atan(Mathf.Sin(i * strikeIncrement) * tanDip);
            
            var currDipRadius = GetRadiusProjection(stereonetRadius, currDipRad);

            curvePoints[i] = ConvertPolarToCartesian(currStrikeRad, currDipRadius);
        }
        
        return curvePoints;
    }

    /// <summary>
    /// Returns the 2D length of the given dip/plunge and stereonet radius
    /// </summary>
    /// <param name="stereonetRadius">Radius of the stereonet</param>
    /// <param name="radians">Degree of the dip/plunge in radians</param>
    /// <returns> The radius in 2D space </returns>
    private static float GetRadiusProjection(float stereonetRadius, float radians)
    {
        // https://run.unl.pt/bitstream/10362/4500/1/CT_11_18.pdf pg.8 (Section I.2.2)
        return Mathf.Sqrt(2f) * stereonetRadius * Mathf.Sin(Mathf.PI * 0.25f - radians * 0.5f);
    }
    
    /// <summary>
    /// Returns a cartesian coordinate from the origin point with the given polar coordinates
    /// </summary>
    /// <param name="angle">Polar coordinate's angle in radians</param>
    /// <param name="radius">Polar coordinate's radius</param>
    private static Vector2 ConvertPolarToCartesian(float angle, float radius)
    {
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);
        
        return new Vector2(x, y);
    }
}
