using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MathNet.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TwoDimensionalStereonetTesting : MonoBehaviour
{
    private float _stereonetRadius = 0.5f * Stereonet2D.STEREONET_IMAGE_RADIUS_PERCENTAGE;

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
    
    [Header("Misc")]
    [SerializeField] private RawImage PoleImagePrefab;
    [SerializeField] private UILineRenderer LineRenderer;

    private void OnValidate()
    {
        UpdatePolePosition();
        UpdatePlanePosition();
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
}
