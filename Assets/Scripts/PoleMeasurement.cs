using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleMeasurement
{
    public Vector3 Normal;
    public bool IsOverturned;
    public GameObject StereonetPointObj;

    public PoleMeasurement(Vector3 normal, bool isOverturned, GameObject stereonetPointObj)
    {
        Normal = normal;
        IsOverturned = isOverturned;
        StereonetPointObj = stereonetPointObj;
    }
}
