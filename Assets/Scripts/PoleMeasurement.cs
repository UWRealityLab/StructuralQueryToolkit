using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PoleMeasurement
{
    public Vector3 Position;
    public bool IsOverturned;

    public PoleMeasurement(Vector3 position, bool isOverturned)
    {
        this.Position = position;
        this.IsOverturned = isOverturned;
    }
    
}
