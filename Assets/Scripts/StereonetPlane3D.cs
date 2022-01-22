using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereonetPlane3D : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    [SerializeField] private LineRenderer lineRenderer;

    private static readonly int _colorProperty = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetColor(Color color)
    {
        _meshRenderer.material.SetColor(_colorProperty, color);
        
        var lineColor = new Color(color.r, color.g, color.b, 1f);
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    public void SetNormal(Vector3 normal, Transform modelTransform)
    {
        transform.rotation = Quaternion.LookRotation(modelTransform.TransformDirection(normal), modelTransform.up);
    }
}
