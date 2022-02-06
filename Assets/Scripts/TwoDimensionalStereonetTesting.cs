using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Complex = System.Numerics.Complex;

public class TwoDimensionalStereonetTesting : MonoBehaviour
{
    private float _stereonetRadius = 0.5f * Stereonet2D.STEREONET_IMAGE_RADIUS_PERCENTAGE;

    [Range(-90f, 90f), Tooltip("The diagrams I'm using have a different strike offset than ours")]
    public float StrikeOffset = -90f;

    [SerializeField] private int numPlaneLineCount = 50;
    
    // Pole debug
    [Header("Pole Debug")] 
    [Range(0f, 360f)]
    public float Trend;
    [Range(0f, 90f)]
    public float Plunge;
    
    // Plane debug
    [Header("Plane Debug")] 
    [Range(0f, 360f)]
    public float Strike;
    [Range(0f, 90f)]
    public float Dip;

    [Header("Multiple Poles and Fold Axis")]
    public Vector2[] PolesStrikeDip;
    [SerializeField] private UILineRenderer FoldAxisLineRenderer;
    [SerializeField] private List<RectTransform> polePointTrans;

    [Header("Misc")]
    [SerializeField] private RawImage PoleImagePrefab;
    [SerializeField] private UILineRenderer LineRenderer;

    private void OnValidate()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        
        UpdatePolePosition();
        UpdatePlanePosition();
        UpdateFoldAxis();
    }

    private void UpdatePolePosition()
    {
        var polePos = TwoDimensionalStereonetUtils.GetPolePosition(_stereonetRadius, Trend, Plunge);
        PoleImagePrefab.rectTransform.anchoredPosition = polePos;
    }
    
    private void UpdatePlanePosition()
    {
        var curvePoints = TwoDimensionalStereonetUtils.GetPlaneLinePoints(_stereonetRadius, Strike, Dip, numPlaneLineCount);
        LineRenderer.Points = curvePoints;
    }

    private void UpdateFoldAxis()
    {
        if (polePointTrans == null)
        {
            polePointTrans = new List<RectTransform>();
        }
        
        while (PolesStrikeDip.Length > polePointTrans.Count)
        {
            polePointTrans.Add(Instantiate(PoleImagePrefab, transform).GetComponent<RectTransform>());
        }
        while (PolesStrikeDip.Length < polePointTrans.Count)
        {
            Destroy(polePointTrans[0].gameObject);
            polePointTrans.RemoveAt(0);
        }
        
        if (PolesStrikeDip.Length <= 2)
        {
            return;
        }
        
        var debugPolePositionsMatrix = Matrix<float>.Build.Dense(PolesStrikeDip.Length, 2);

        var lonLatArr = new Vector2[PolesStrikeDip.Length * 2];

        var idx = 0;
        foreach (var strikeDipPair in PolesStrikeDip)
        {
            // Set position
            StereonetUtils.CalculateTrendAndPlunge(strikeDipPair.x + StrikeOffset, strikeDipPair.y, out var trend, out var plunge);
            polePointTrans[idx].anchoredPosition = TwoDimensionalStereonetUtils.GetPolePosition(_stereonetRadius, trend, plunge);
            // DEBUG just want to record poles cartesian positions
            debugPolePositionsMatrix.SetRow(idx, new [] {polePointTrans[idx].anchoredPosition.x, polePointTrans[idx].anchoredPosition.y});
            
            // Record strike and dip to find fold axis
            TwoDimensionalStereonetUtils.ConvertToLonLat(strikeDipPair.x, strikeDipPair.y, out var lon, out var lat);
            lonLatArr[idx++] = new Vector2(lon, lat);
        }
        //print(debugPolePositionsMatrix.ToString());
        
        foreach (var strikeDipPair in PolesStrikeDip)
        {
            var (lon, lat) = TwoDimensionalStereonetUtils.AntipodeStrikeDip(strikeDipPair.x, strikeDipPair.y);
            lonLatArr[idx++] = new Vector2(lon, lat);
        }

        var cartMatrix = Matrix<float>.Build.Dense(PolesStrikeDip.Length * 2, 3);

        for (int i = 0; i < lonLatArr.Length; i++)
        {
            var (lon, lat) = (lonLatArr[i].x, lonLatArr[i].y);
            var cartesian = TwoDimensionalStereonetUtils.SphToCart(lon, lat);
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
        
        print(evd.EigenValues);
        print(evd.EigenVectors);
        
        // Get the smallest eigenvector and convert that from lat/lon to strike/dip
        // This is the fold axis plane
        var smallestEigenVec = new Vector3(minEigenVec[0], minEigenVec[1], minEigenVec[2]);
        TwoDimensionalStereonetUtils.CartToSph(smallestEigenVec, out var foldAxisLon, out var foldAxisLat);
        TwoDimensionalStereonetUtils.SphToStrikeDip(foldAxisLon, foldAxisLat, out var foldAxisStrike, out var foldAxisDip);
        
        print($"{smallestEigenVec.x}, {smallestEigenVec.y}, {smallestEigenVec.z}");
        print($"{foldAxisStrike}, {foldAxisDip}");

        var curvePoints = TwoDimensionalStereonetUtils.GetPlaneLinePoints(_stereonetRadius, foldAxisStrike - StrikeOffset, foldAxisDip, numPlaneLineCount);
        FoldAxisLineRenderer.Points = curvePoints;
    }
}
