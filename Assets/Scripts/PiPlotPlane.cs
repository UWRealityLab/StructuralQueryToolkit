using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created when a planar measurement is combined.
/// Stores all of its dependent points and triangle. 
/// </summary>
public class PiPlotPlane : Measurement
{
    public Transform plane;

    public float strike;
    public float dip;

    public LineRenderer lineRenderer;

    public Queue<Transform> combinedPlanes; // Plane in the world
    public Queue<PiPlotPlane> combinedPlaneLines;

    [SerializeField] Transform leftCorner;
    [SerializeField] Transform rightCorner;


    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetForward(Vector3 forward)
    {
        plane.forward = forward;
        UpdateLine();
    }

    public void UpdateLine()
    {
        int iters = 50;
        lineRenderer.positionCount = 0;
        for (int i = 0; i < iters; i++)
        {
            Vector3 pos = Vector3.Lerp(leftCorner.position, rightCorner.position, Mathf.SmoothStep(0f, 1f, (float)i / (float)(iters - 1)));

            RaycastHit hit;
            if (Physics.Raycast(pos, -plane.up, out hit, 10f, 1 << PlanePlotting.stereonetLayer)) {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z)); // Offset in Y to account for z fighting
            }
        }
        StereonetUtils.CalculateStrikeAndDip(plane.forward, out strike, out dip);
    }

    public void ConvertToCombinedPlane()
    {
        isCombined = true;
        combinedPlanes = new Queue<Transform>();
        combinedPlaneLines = new Queue<PiPlotPlane>();
    }

    public void AddCombinedPlane(Transform plane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlanes.Enqueue(plane);

    }

    public void AddCombinedPLaneLine(PiPlotPlane piPlotPLane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlaneLines.Enqueue(piPlotPLane);
    }
}

public class PiPlotPlaneCombined : PiPlotPlane
{
    public string groupName;

    public Queue<Transform> combinedPlanes;
    public Queue<PiPlotPlane> combinedPlaneLines;

    public void Start()
    {
        combinedPlanes = new Queue<Transform>();
        combinedPlaneLines = new Queue<PiPlotPlane>();
    }

    public void AddCombinedPlane(Transform plane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlanes.Enqueue(plane);

    }

    public void AddCombinedPLaneLine(PiPlotPlane piPlotPLane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlaneLines.Enqueue(piPlotPLane);
    }
}