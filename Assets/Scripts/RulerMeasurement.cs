using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RulerMeasurement : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform textTransform;
    public TextMeshProUGUI text;

    private float length;

    private Stack<Transform> rulerPoints;
    private static readonly int colorID = Shader.PropertyToID("Color");


    // Start is called before the first frame update
    void Awake()
    {
        text.isOverlay = true;
        length = 0;
        text.text = "";
        rulerPoints = new Stack<Transform>();
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AddPoint(Vector3 point, Vector3 normal)
    {
        Transform rulerPoint = Instantiate(RulerPlotting.instance.rulerPointPrefab, point, Quaternion.identity).transform;
        rulerPoint.localScale *= Settings.instance.ObjectScaleMultiplier;
        rulerPoint.up = normal;

        var distanceAdded = 0f;
        if (lineRenderer.positionCount > 0)
        {
            var secondToLastPoint = rulerPoints.Peek().position;
            distanceAdded = Vector3.Distance(secondToLastPoint, rulerPoint.position);
        }
        
        rulerPoints.Push(rulerPoint);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, rulerPoint.position);
        UpdateText(distanceAdded);
    }

    public void Undo()
    {
        Destroy(rulerPoints.Pop().gameObject);
        lineRenderer.positionCount--;

        if (lineRenderer.positionCount >= 2)
        {
            var deltaLength = -Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), lineRenderer.GetPosition(lineRenderer.positionCount - 2));
            UpdateText(deltaLength);
        }
        else if (lineRenderer.positionCount == 1)
        {
            length = 0;
            text.text = "";
            UpdateTextPositionLast();
        }
        else
        {
            RulerPlotting.instance.RemoveRuler();
        }
    }

    // Places the text in the centroid of all the points
    public void UpdateText(float deltaLength)
    {
        UpdateTextPositionLast();
        AddLength(deltaLength);
    }

    /// <summary>
    /// Updates the text position to the centroid of all its points
    /// </summary>
    private void UpdateTextPositionCentroid()
    {
        var centroid = Vector3.zero;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            centroid += lineRenderer.GetPosition(i);
        }

        centroid /= lineRenderer.positionCount;

        text.transform.position = centroid;
    }

    /// <summary>
    /// Updates the text position to the last node
    /// </summary>
    /// <returns></returns>
    private void UpdateTextPositionLast()
    {
        textTransform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }

    private void AddLength(float len)
    {
        length += len;
        text.text = string.Format("{0} m", length.ToString("F2"));
    }

    private void OnDestroy()
    {
        foreach (var point in rulerPoints)
        {
            if (point)
            {
                Destroy(point.gameObject);
            }
        }
    }

    public void SetSize(float size)
    {
        text.fontSize = size;
    }

    public int GetNumPoints()
    {
        return rulerPoints.Count;
    }

    public Vector3 GetLatestPoint()
    {
        return rulerPoints.Peek().position;
    }

    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        foreach (var point in rulerPoints)
        {
            point.GetComponent<MeshRenderer>().material.SetColor(colorID, color);
        }

        text.color = color;
    }
    
    public void Destroy()
    {
        foreach (var point in rulerPoints)
        {
            GameObject.Destroy(point.gameObject);
        }
        Destroy(gameObject);
    }

    public void SetVisibilityState(bool state)
    {
        foreach (var point in rulerPoints)
        {
            point.gameObject.SetActive(state);
        }

        lineRenderer.enabled = state;
        text.gameObject.SetActive(state);
    }

}
