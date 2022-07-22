using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMeasurementGroup : MeasurementsGroup
{
    private List<(float, float)> _trendAndPlungeArr;
    
    public List<(float, float)> TrendAndPlungeArr { 
        get
        {
            _trendAndPlungeArr ??= new List<(float, float)>();
            return _trendAndPlungeArr;
        }
    }

    
    public (float, float) AverageTrendAndPlunge()
    {
        _trendAndPlungeArr ??= new List<(float, float)>();
        
        var avgTrend = 0f;
        var avgPlunge = 0f;

        foreach (var pair in _trendAndPlungeArr)
        {
            avgTrend += pair.Item1;
            avgPlunge += pair.Item2;
        }

        return (avgTrend / _trendAndPlungeArr.Count, avgPlunge / _trendAndPlungeArr.Count);
    }

}