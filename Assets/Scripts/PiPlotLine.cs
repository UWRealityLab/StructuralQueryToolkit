using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created when a linear measurement is combined.
/// Stores all of its dependent world and stereonet lines
/// </summary>
public class PiPlotLine : Measurement
{
    public float trend;
    public float plunge;
    public Queue<Transform> combinedWorldLines;
    public Queue<PiPlotLine> combinedStereonetPoints;

    public void SetData(Vector3 normal)
    {
        // Setting the forward is visually irrelevant since the object is a
        // sphere, but it is used later when calculating the average trend and plunge
        transform.forward = normal;

        StereonetUtils.CalculateTrendAndPlunge(normal, out trend, out plunge);
    }

    public void ConvertToCombinedLine()
    {
        isCombined = true;
        combinedWorldLines = new Queue<Transform>();
        combinedStereonetPoints = new Queue<PiPlotLine>();
    }

    public void AddCombinedWorldLine(Transform plane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined line");
        }

        combinedWorldLines.Enqueue(plane);

    }

    public void AddCombinedLinearLine(PiPlotLine piPlotLine)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined line");
        }

        combinedStereonetPoints.Enqueue(piPlotLine);
    }

}


/// Very poorly designed (a waste of memory since every stereonet linear point
/// will have this component, and only the one being combined is using it).

