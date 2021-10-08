using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMeasurementGroup : MeasurementsGroup
{
    private void Awake()
    {
        trendAndPlungeArr = new List<(float, float)>();
    }

    public List<(float, float)> trendAndPlungeArr;
    
    public (float, float) AverageTrendAndPlunge()
    {
        var avgTrend = 0f;
        var avgPlunge = 0f;

        foreach (var pair in trendAndPlungeArr)
        {
            avgTrend += pair.Item1;
            avgPlunge += pair.Item2;
        }

        return (avgTrend / trendAndPlungeArr.Count, avgPlunge / trendAndPlungeArr.Count);
    }

}