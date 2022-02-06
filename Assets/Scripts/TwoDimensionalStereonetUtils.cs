using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

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

        if (dip.AlmostEqual(90f, 0.01f))
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

    
    /**

    Copyright (c) 2021 University of Washington

    ---

    Copyright (c) 2012 Free Software Foundation

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
    of the Software, and to permit persons to whom the Software is furnished to do
    so, subject to the following conditions:

     */
    
    public static void GetFoldAxis(Vector2[] polePointStrikeDips, out float strike, out float dip, out Vector3 normal)
    {
        var lonLatArr = new Vector2[polePointStrikeDips.Length * 2];

        var idx = 0;
        foreach (var strikeDipPair in polePointStrikeDips)
        {
            // Record strike and dip to find fold axis
            ConvertToLonLat(strikeDipPair.x, strikeDipPair.y, out var lon, out var lat);
            lonLatArr[idx++] = new Vector2(lon, lat);
        }
        
        foreach (var strikeDipPair in polePointStrikeDips)
        {
            var (lon, lat) = AntipodeStrikeDip(strikeDipPair.x, strikeDipPair.y);
            lonLatArr[idx++] = new Vector2(lon, lat);
        }

        var cartMatrix = Matrix<float>.Build.Dense(polePointStrikeDips.Length * 2, 3);

        for (int i = 0; i < lonLatArr.Length; i++)
        {
            var (lon, lat) = (lonLatArr[i].x, lonLatArr[i].y);
            var cartesian = SphToCart(lon, lat);
            cartMatrix.SetRow(i, new [] {cartesian.x, cartesian.y, cartesian.z});
        }

        var covMatrix = Stereonet2D.GetCovarianceMatrix(cartMatrix);
        
        var evd = covMatrix.Evd(Symmetricity.Asymmetric);

        var eigenVals = evd.EigenValues.AsArray();
        var eigenVecs = evd.EigenVectors.ToColumnArrays();
        
        var minEigenVal = double.MaxValue;
        float[] minEigenVec = Array.Empty<float>();
        
        for (int i = 0; i < eigenVals.Length; i++)
        {
            var currEigenVal = eigenVals[i];

            if (currEigenVal.Real < minEigenVal)
            {
                minEigenVal = currEigenVal.Real;
                minEigenVec = eigenVecs[i];
            }
        }
        
        // Get the smallest eigenvector and convert that from lat/lon to strike/dip. This is the fold axis plane
        normal = new Vector3(minEigenVec[0], minEigenVec[1], minEigenVec[2]);
        CartToSph(normal, out var foldAxisLon, out var foldAxisLat);
        SphToStrikeDip(foldAxisLon, foldAxisLat, out strike, out dip);
    }

    public static void ConvertToLonLat(float strikeDeg, float dipDeg, out float lon, out float lat)
    {
        var cond = dipDeg > 90f;
        dipDeg = cond ? 180f - dipDeg : dipDeg;
        strikeDeg = cond ? strikeDeg + 180f : strikeDeg;

        lon = -dipDeg * Mathf.Deg2Rad;
        lat = 0f;
        
        var rotatedSphCoord = Quaternion.AngleAxis(strikeDeg, -Vector3.right) * SphToCart(lon, lat);

        CartToSph(rotatedSphCoord, out lon, out lat);
    }
    
    public static Vector3 SphToCart(float lon, float lat)
    {
        var x = Mathf.Cos(lat) * Mathf.Cos(lon);
        var y = Mathf.Cos(lat) * Mathf.Sin(lon);
        var z = Mathf.Sin(lat);
        return new Vector3(x, y, z);
    }

    public static void CartToSph(Vector3 vec, out float lon, out float lat)
    {
        var radius = math.length(vec);
        lat = Mathf.Asin(vec.z / radius); // TODO y or z?
        lon = Mathf.Atan2(vec.y, vec.x);
    }

    public static void SphToStrikeDip(float lon, float lat, out float strike, out float dip)
    {
        var xyz = SphToCart(lon, lat);
        
        var bearing = Mathf.Atan2(xyz.z, xyz.y) * Mathf.Rad2Deg;

        var radius = math.length(xyz);
        radius = radius == 0f ? Mathf.Epsilon : radius;
        var plunge = Mathf.Asin(xyz.x / radius) * Mathf.Rad2Deg;

        bearing = 90f - bearing;
        bearing = bearing < 0f ? bearing + 360f : bearing;

        if (plunge < 0f)
        {
            plunge *= -1f;
            bearing -= 180f;
            bearing = bearing < 0f ? bearing + 360f : bearing;
        }

        strike = bearing + 90f;
        strike = strike >= 360f ? strike - 360f : strike;
        dip = 90f - plunge;
    }

    public static (float, float) AntipodeStrikeDip(float strike, float dip)
    {
        ConvertToLonLat(strike, dip, out var lon, out var lat);
        return Antipode(lon, lat);
    }

    public static (float, float) Antipode(float lon, float lat)
    {
        var xyz = SphToCart(lon, lat);
        CartToSph(-xyz, out var newLon, out var newLat);
        return (newLon, newLat);
    }
}
