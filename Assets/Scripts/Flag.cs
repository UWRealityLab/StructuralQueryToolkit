using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public MeshRenderer flagMeshRenderer;
    public Stereonet stereonet;
    
    public PoleMeasurement PoleMeasurement;
    public RectTransform StereonetPole2D;
    public StereonetPole3D StereonetPole3D;
    
    /// <summary>
    /// If flags are moved dynamically, then their poles need to be updated
    /// </summary>
    public void UpdateStereonet()
    {
        stereonet.ChangePoleData(this);
    }
}
