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

    //[DrawIf("showElevationData", true, DrawIfAttribute.DisablingType.DontDraw)]
    public float elevationBias = 0f;

    [Header("Random Sampling (Pole measurements only)")]
    public bool randomSamplingRadius = false;

    //[DrawIf("randomSamplingRadius", true, DrawIfAttribute.DisablingType.DontDraw)]
    [Tooltip("Note that samples can be invalid and discarded")]
    public int maxSampleCount = 20;

    //[DrawIf("randomSamplingRadius", true, DrawIfAttribute.DisablingType.DontDraw)]
    [Tooltip("The radius of the random sampling (meters)")]
    public float samplingRadius = 1f;

    [Header("Scaling")] 
    [Tooltip("The scale of measurement tools (flags, measurement spheres, etc.)")]
    public float ObjectScaleMultiplier = 1f;
    public float JetpackVerticalSpeed = 1f;
    public float JetpackMovementSpeed = 1f;

    void Awake()
    {
        instance = this;
        elevationBias = 0f;
    }

    public void SetAudio(bool isEnabled)
    {
        AudioListener.pause = isEnabled;
    }

    private void OnValidate()
    {
        if (samplingRadius < Mathf.Epsilon)
        {
            samplingRadius = 0f;
        }
        if (maxSampleCount < 0)
        {
            maxSampleCount = 0;
        }
    }
}
