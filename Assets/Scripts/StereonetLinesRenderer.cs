using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class StereonetLinesRenderer : MonoBehaviour
{
    public float Radius = 1f;
    public int NumPointsPerArc = 30;

    [SerializeField] private List<LineRenderer> lines;
    
    private static readonly float DEGREES = Mathf.PI;
    private static readonly float DEGREES_OFFSET = Mathf.PI * 0.5f;

    private void OnValidate()
    {
        Assert.IsTrue(lines.Count >= 34);

        var initialPoints = new Vector3[NumPointsPerArc];
        for (int i = 0; i < NumPointsPerArc; i++)
        {
            var currAnlge = DEGREES * ((float) i / (NumPointsPerArc - 1)) + DEGREES_OFFSET;
            var x = Mathf.Cos(currAnlge);
            var y = Mathf.Sin(currAnlge);
            initialPoints[i] = new Vector3(x, 0f, y) * Radius;
        }
        for (int i = 0; i < 17; i++)
        {
            var currLine = lines[i];
            currLine.positionCount = NumPointsPerArc;
            currLine.SetPositions(initialPoints);
            currLine.transform.localRotation = Quaternion.Euler(0f, 0f, (i + 1) * 10f);
        }

        return;
        for (int i = 0; i < 17; i++)
        {
            var lineIndex = i + 17;
            var currLine = lines[lineIndex];
            currLine.positionCount = NumPointsPerArc;
            currLine.SetPositions(initialPoints);
            
            currLine.transform.rotation = Quaternion.Euler(0f, 90f, 90f);
            var scale = Mathf.Sin(i * 10f * Mathf.Deg2Rad);
            currLine.transform.localScale = new Vector3(scale, scale, scale);
            currLine.transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(-Radius, Radius, (float) i / (17 - 1)));
        }
    }
}
