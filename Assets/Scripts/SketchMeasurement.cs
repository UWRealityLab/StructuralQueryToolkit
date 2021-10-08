using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SketchMeasurement : MonoBehaviour
{
    public LineRenderer lineRenderer;
    
    private MeshCollider MeshCollider;
    private Mesh lineMesh;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        MeshCollider = GetComponent<MeshCollider>();
        lineMesh = new Mesh();
    }

    public void AddPoint(Vector3 point, Vector3 normal)
    {
        if (lineRenderer.positionCount > 0)
        {
            var secondToLastPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            if (Vector3.Distance(point, secondToLastPoint) < 0.1f * Settings.instance.ObjectScaleMultiplier)
            {
                return;
            }
        }
        
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);
    }

    public void FinishLine(Camera currCamera)
    {
        //lineRenderer.Simplify(lineRenderer.startWidth / 2f);
        lineRenderer.BakeMesh(lineMesh, currCamera, false);
        MeshCollider.sharedMesh = lineMesh;
    }
}