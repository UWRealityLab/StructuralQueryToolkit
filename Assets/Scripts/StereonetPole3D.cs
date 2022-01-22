using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereonetPole3D : MonoBehaviour
{
    [SerializeField] private Transform startSphere;
    [SerializeField] private Transform endSphere;
    [SerializeField] private LineRenderer lineRenderer;

    private MeshRenderer startSphereMeshRenderer;
    private MeshRenderer endSphereMeshRenderer;
    
    private static readonly int _colorProperty = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        startSphereMeshRenderer = startSphere.GetComponent<MeshRenderer>();
        endSphereMeshRenderer = endSphere.GetComponent<MeshRenderer>();
    }

    public void SetColor(Color color)
    {
        startSphereMeshRenderer.material.SetColor(_colorProperty, color);
        endSphereMeshRenderer.material.SetColor(_colorProperty, color);
        lineRenderer.startColor = new Color(color.r, color.g, color.b, 1f);
        lineRenderer.endColor = new Color(color.r, color.g, color.b, 1f);;
    }
    
    public void SetNormal(Vector3 normal, float radius = 1f)
    {
        var pos = -normal * radius;
        endSphere.localPosition = pos;
        lineRenderer.SetPosition(1, pos);
    }


}
