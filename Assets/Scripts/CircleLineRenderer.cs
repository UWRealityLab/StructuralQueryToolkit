using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class CircleLineRenderer : MonoBehaviour
{
    public float Radius = 1f;

    public int NumPoints = 32;

    private static readonly float DEGREES = Mathf.PI * 2f;
    
    private void OnValidate()
    {
        var lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = NumPoints;
        
        var points = new Vector3[NumPoints];

        for (int i = 0; i < NumPoints; i++)
        {
            var currAngle = DEGREES * ((float) i / (NumPoints - 1));
            var x = Mathf.Cos(currAngle);
            var y = Mathf.Sin(currAngle);
            
            points[i] = new Vector3(x, 0f, y) * Radius;
        }

        lineRenderer.SetPositions(points);
    }
}
