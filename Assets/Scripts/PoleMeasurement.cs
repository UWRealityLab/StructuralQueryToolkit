using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PoleMeasurement
{
    public Vector3 Position;
    public Vector3 Normal;
    public bool IsOverturned;
    public GameObject StereonetPointObj;

    public PoleMeasurement(Vector3 position, Vector3 normal, bool isOverturned, GameObject stereonetPointObj)
    {
        Position = position;
        Normal = normal;
        IsOverturned = isOverturned;
        StereonetPointObj = stereonetPointObj;
    }
}
