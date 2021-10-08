using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMeasurementGroup : MeasurementsGroup
{
    public List<(float, float)> strikeAndDipArr;

    private void Awake()
    {
        strikeAndDipArr = new List<(float, float)>();
    }

    public (float, float) AverageStrikeAndDip()
    {
        var avgStrike = 0f;
        var avgDip = 0f;

        foreach (var pair in strikeAndDipArr)
        {
            avgStrike += pair.Item1;
            avgDip += pair.Item2;
        }

        return (avgStrike / strikeAndDipArr.Count, avgDip / strikeAndDipArr.Count);
    }

}
