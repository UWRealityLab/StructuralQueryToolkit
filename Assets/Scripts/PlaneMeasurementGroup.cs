using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMeasurementGroup : MeasurementsGroup
{
    private List<(float, float)> _strikeAndDipArr;
    
    public List<(float, float)> StrikeAndDipArr { 
        get
        {
            _strikeAndDipArr ??= new List<(float, float)>();
            return _strikeAndDipArr;
        }
    }

    public (float, float) AverageStrikeAndDip()
    {
        _strikeAndDipArr ??= new List<(float, float)>();

        var avgStrike = 0f;
        var avgDip = 0f;

        foreach (var pair in _strikeAndDipArr)
        {
            avgStrike += pair.Item1;
            avgDip += pair.Item2;
        }

        return (avgStrike / _strikeAndDipArr.Count, avgDip / _strikeAndDipArr.Count);
    }

}
