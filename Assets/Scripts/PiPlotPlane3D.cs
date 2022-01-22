using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI.Extensions;

public abstract class PiPlotPlane : Measurement
{
    public float strike { get; protected set; }
    public float dip { get; protected set; }

    public Queue<Transform> combinedPlanes; // Plane in the world
    public Queue<PiPlotPlane> combinedPlaneLines;

    public abstract void SetForward(Vector3 forward);
    public abstract Vector3 GetForwardDirection();

}

/// <summary>
/// Created when a planar measurement is combined.
/// Stores all of its dependent points and triangle. 
/// </summary>
public class PiPlotPlane3D : PiPlotPlane
{
    public Transform plane;

    public LineRenderer lineRenderer;

    [SerializeField] Transform leftCorner;
    [SerializeField] Transform rightCorner;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void SetForward(Vector3 forward)
    {
        plane.forward = forward;
        UpdateLine();
    }

    public override Vector3 GetForwardDirection()
    {
        return plane.forward;
    }

    private void UpdateLine()
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
        StereonetUtils.CalculateStrikeAndDip(plane.forward, out var newStrike, out var newDip);
        strike = newStrike;
        dip = newDip;
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

    public void AddCombinedPLaneLine(PiPlotPlane3D piPlotPLane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlaneLines.Enqueue(piPlotPLane);
    }
}

public class PiPlotPlane2D : PiPlotPlane
{
    public Vector3 forward { get; private set; }

    private UILineRenderer _lineRenderer;
    public UILineRenderer LineRenderer
    {
        get
        {
            if (_lineRenderer)
            {
                return _lineRenderer;
            }

            return GetComponent<UILineRenderer>();
        }
    }

    public UnityEvent<Vector3> OnRotate;

    public Queue<Transform> combinedPlanes; // Plane in the world
    public Queue<StereonetPlane3D> stereonetPlanes3D;

    private void Awake()
    {
        _lineRenderer = GetComponent<UILineRenderer>();
        OnRotate ??= new UnityEvent<Vector3>();
    }

    public override void SetForward(Vector3 forward)
    {
        this.forward = forward;
        UpdateLine();
    }

    public override Vector3 GetForwardDirection()
    {
        return forward;
    }

    private void UpdateLine()
    {
        StereonetUtils.CalculateStrikeAndDip(forward, out var newStrike, out var newDip);
        strike = newStrike;
        dip = newDip;
        LineRenderer.Points = TwoDimensionalStereonetUtils.GetPlaneLinePoints(Stereonet2D.STEREONET_IMAGE_RADIUS, strike, dip, Stereonet2D.NUM_CURVE_POINTS);

        OnRotate ??= new UnityEvent<Vector3>();
        OnRotate.Invoke(forward);
    }

    public void ConvertToCombinedPlane()
    {
        isCombined = true;
        combinedPlanes = new Queue<Transform>();
        combinedPlaneLines = new Queue<PiPlotPlane>();
        stereonetPlanes3D = new Queue<StereonetPlane3D>();
    }

    public void AddCombinedPlane(Transform plane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlanes.Enqueue(plane);

    }

    public void AddCombinedPLaneLine(PiPlotPlane2D piPlotPLane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlaneLines.Enqueue(piPlotPLane);
    }
}

public class PiPlotPlane3DCombined : PiPlotPlane3D
{
    public string groupName;

    public Queue<Transform> combinedPlanes;
    public Queue<PiPlotPlane3D> combinedPlaneLines;

    public void Start()
    {
        combinedPlanes = new Queue<Transform>();
        combinedPlaneLines = new Queue<PiPlotPlane3D>();
    }

    public void AddCombinedPlane(Transform plane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlanes.Enqueue(plane);

    }

    public void AddCombinedPLaneLine(PiPlotPlane3D piPlotPLane)
    {
        if (!isCombined)
        {
            Debug.LogError("Not combined plane");
        }

        combinedPlaneLines.Enqueue(piPlotPLane);
    }
}