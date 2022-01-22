using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created when a linear measurement is combined.
/// Stores all of its dependent world and stereonet lines
/// </summary>
public class PiPlotLine : Measurement
{
    public Vector3 normal;
    public float trend;
    public float plunge;
    public Queue<Transform> combinedWorldLines;
    public Queue<StereonetPole3D> combinedStereonetLines3D;
    public Queue<PiPlotLine> combinedStereonetPoints;

    public void SetData(Vector3 normal)
    {
        this.normal = normal;

        StereonetUtils.CalculateTrendAndPlunge(normal, out trend, out plunge);
    }

    public void ConvertToCombinedLine()
    {
        isCombined = true;
        combinedWorldLines = new Queue<Transform>();
        combinedStereonetPoints = new Queue<PiPlotLine>();
        combinedStereonetLines3D = new Queue<StereonetPole3D>();
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