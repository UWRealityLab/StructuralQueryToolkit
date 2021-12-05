using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    [Header("Elevation")]
    [Tooltip("Enable this if you have set your model to its proper elevation using the Altitude Markers objects")]
    public bool showElevationData;
    
    [HideInInspector] public bool isReadOnly = true;
    [DrawIf("isReadOnly", false, DrawIfAttribute.DisablingType.ReadOnly)]
    public float elevationBias;

    [Header("Random Sampling (Pole measurements only)")]
    public bool randomSamplingRadius = false;

    [DrawIf("randomSamplingRadius", true, DrawIfAttribute.DisablingType.ReadOnly)]
    [Tooltip("Note that samples can be invalid and discarded"), Min(0)]
    public int maxSampleCount = 20;

    [DrawIf("randomSamplingRadius", true, DrawIfAttribute.DisablingType.ReadOnly)]
    public float samplingRadius = 1f;

    [Header("Scaling")] 
    [Tooltip("The scale of measurement tools (flags, measurement spheres, etc.)")]
    [Min(0f)]
    public float ObjectScaleMultiplier = 1f;
    [Min(0f)]
    public float JetpackVerticalSpeed = 1f;
    [Min(0f)]
    public float JetpackMovementSpeed = 1f;

    void Awake()
    {
        instance = this;
        if (showElevationData)
        {
            AltitudeMarker.UpdateAltitudeBias();
        }
        else
        {
            elevationBias = 0f;
        }
    }

    private void Start()
    {
        //Application.targetFrameRate = 60;
    }

    public void SetAudio(bool isEnabled)
    {
        AudioListener.pause = isEnabled;
    }
    

    private void OnValidate()
    {
        if (maxSampleCount < 0)
        {
            maxSampleCount = 0;
        }
        samplingRadius = Mathf.Max(samplingRadius, 0f);
        
        if (showElevationData)
        {
            AltitudeMarker.UpdateAltitudeBias();
        }
        else
        {
            elevationBias = 0f;
        }
    }
}
