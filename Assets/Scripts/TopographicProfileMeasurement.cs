using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TopographicProfileMeasurement : MonoBehaviour
{
    public LinkedList<Transform> markerPoints;
    public LineRenderer line;
    private LinkedList<TopoProfileGraphPoint> graphPoints;

    public float TotalDistance; // NOTE: this does not represent the actual distance of all the marker points due to the profile lock mechanic
    
    public float MinHeight;
    public float MaxHeight;

    public Vector3 PlanePos;
    public Vector3 PlaneDir;
    
    public bool IsProfileLocked = true;
    private bool _isVisible = true;
    private static readonly int _colorShaderProp = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        markerPoints = new LinkedList<Transform>();
        graphPoints = new LinkedList<TopoProfileGraphPoint>();

        MinHeight = float.MaxValue;
        MaxHeight = float.MinValue;
    }

    /// <summary>
    /// Registers a profile marker where the y point will be the marker y positions, and the x point will be the
    /// distance travelled from the last marker
    /// </summary>
    public void AddPointProfileUnlocked(RaycastHit hit)
    {
        var elevation = Settings.instance.showElevationData ? hit.point.y + Settings.instance.elevationBias : hit.point.y;
        
        if (markerPoints.Count == 1)
        {
            // First case
            graphPoints.AddFirst(new TopoProfileGraphPoint(0f, elevation, 0f, MaxHeight, MinHeight));
            MinHeight = Mathf.Min(MinHeight, elevation);
            MaxHeight = Mathf.Max(MaxHeight, elevation);
            return;
        }
        
        var prevPos = markerPoints.First.Next.Value.position;

        PlanePos = prevPos;

        var dist = Vector3.Distance(hit.point, prevPos);

        graphPoints.AddFirst(new TopoProfileGraphPoint(TotalDistance + dist, elevation, TotalDistance, MaxHeight, MinHeight));

        TotalDistance += dist;
        MinHeight = Mathf.Min(MinHeight, elevation);
        MaxHeight = Mathf.Max(MaxHeight, elevation);

        UpdateProfilePlane();
        
        if (!_isVisible)
        {
            Show();
        }
    }

    /// <summary>
    /// Registers a profile marker where the y point is the marker y positions, and the x point is the 1D distance
    /// from the marker position to the start of the profile plane 
    /// </summary>
    public void AddPointProfileLocked(RaycastHit hit)
    {
        // The start position of the plane is the last point in the markerPoints stack
        //var startPos = markerPoints.Last.Value.position;
        var startPos = PlanePos;
        startPos.y = 0f;

        var elevation = Settings.instance.showElevationData ? hit.point.y + Settings.instance.elevationBias : hit.point.y;

        var newPointPos = hit.point;
        newPointPos.y = 0f;
        
        var dist = Vector3.Distance(startPos, newPointPos) - TotalDistance; // The actual 1D distance along the plane

        graphPoints.AddFirst(new TopoProfileGraphPoint(TotalDistance + dist, elevation, TotalDistance, MaxHeight, MinHeight));
        
        MinHeight = Mathf.Min(MinHeight, elevation);
        MaxHeight = Mathf.Max(MaxHeight, elevation);
        TotalDistance = Mathf.Max(TotalDistance, TotalDistance + dist);

        if (!_isVisible)
        {
            Show();
        }
    }
    
    public void Undo()
    {
        if (markerPoints.Count <= 0)
        {
            return;
        }
        var obj = markerPoints.First.Value;
        Destroy(obj.gameObject);
        markerPoints.RemoveFirst();

        var removedGraphPoint = graphPoints.First.Value;
        TotalDistance = removedGraphPoint.PrevTotalDistance;
        MinHeight = removedGraphPoint.PrevMinHeight;
        MaxHeight = removedGraphPoint.PrevMaxHeight;

        graphPoints.RemoveFirst();
    }
    
    private void UpdateProfilePlane()
    {
        var latestPos = markerPoints.First.Value.position;
        var prevPos = markerPoints.First.Next.Value.position;
        PlanePos = prevPos;
        PlaneDir = prevPos - latestPos;
    }

    public void Show()
    {
        foreach (var markerPoint in markerPoints)
        {
            markerPoint.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        foreach (var markerPoint in markerPoints)
        {
            markerPoint.gameObject.SetActive(false);
        }
    }

    public bool GetVisibility()
    {
        return _isVisible;
    }
    
    public void SetVisibilityState(bool state)
    {
        _isVisible = state;
        if (state)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void SetGlobalVisibility(bool state)
    {
        if (!_isVisible) return;
        
        if (state)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void SetColor(Color color)
    {
        foreach (var marker in markerPoints)
        {
            marker.GetComponentInChildren<MeshRenderer>().material.SetColor(_colorShaderProp, color);
        }
    }

    public void SetMarkerSize(float size)
    {
        foreach (var marker in markerPoints)
        {
            marker.localScale = new float3(size) * Settings.instance.ObjectScaleMultiplier;
        }
    }

    public Vector2[] GetGraphPoints(Vector2 graphDimensions)
    {
        var output = new Vector2[markerPoints.Count];

        var scaling = Mathf.Max(TotalDistance, Mathf.Abs(MaxHeight - MinHeight));
        var aspectRatio = graphDimensions.x / graphDimensions.y; // The X and Y axis need to be 1:1
        
        var i = 0;
        foreach (var graphPoint in graphPoints)
        {
            //output[i] = new Vector2(graphPoint.Distance / TotalDistance, (graphPoint.Elevation - MinHeight) / Mathf.Abs(MaxHeight - MinHeight));
            output[i] = new Vector2(graphPoint.Distance / scaling / aspectRatio, (graphPoint.Elevation - MinHeight) / scaling);
            i++;
        }

        return output;
    }
    
    public void GetGraphPoints(Vector2 graphDimensions, out Vector2[] output, out float maxY, out float maxX)
    {
        output = new Vector2[markerPoints.Count];
        maxY = float.MinValue;
        maxX = float.MinValue;

        var scaling = Mathf.Max(TotalDistance, Mathf.Abs(MaxHeight - MinHeight));
        var aspectRatio = graphDimensions.x / graphDimensions.y; // The X and Y axis need to be 1:1
        
        var i = 0;
        foreach (var graphPoint in graphPoints)
        {
            //output[i] = new Vector2(graphPoint.Distance / TotalDistance, (graphPoint.Elevation - MinHeight) / Mathf.Abs(MaxHeight - MinHeight));
            output[i] = new Vector2(graphPoint.Distance / scaling / aspectRatio, (graphPoint.Elevation - MinHeight) / scaling);

            maxX = Mathf.Max(maxX, output[i].x);
            maxY = Mathf.Max(maxY, output[i].y);
            
            i++;
        }
    }
    
    public Vector2[] GetRawGraphPoints()
    {
        var output = new Vector2[markerPoints.Count];

        var i = 0;
        foreach (var graphPoint in graphPoints)
        {
            output[i] = new Vector2(graphPoint.Distance, graphPoint.Elevation);
            i++;
        }

        return output;
    }



    public void Destroy()
    {
        Destroy(gameObject);
    }

    private struct TopoProfileGraphPoint
    {
        public float Distance;
        public float Elevation;
        public float PrevTotalDistance;
        public float PrevMaxHeight;
        public float PrevMinHeight;

        public TopoProfileGraphPoint(float distance, float elevation, float prevTotalDistance, float prevMaxHeight, float prevMinHeight)
        {
            Distance = distance;
            Elevation = elevation;
            PrevTotalDistance = prevTotalDistance;
            PrevMaxHeight = prevMaxHeight;
            PrevMinHeight = prevMinHeight;
        }
    }
}

