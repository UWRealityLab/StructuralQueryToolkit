using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class StereonetMeasuringTool : MonoBehaviour
{
    [Serializable]
    public enum StereonetMeasurementType
    {
        Pole,
        Line,
        Plane,
        None
    }

    [SerializeField] private StereonetMeasurementType _stereonetMeasurementType = StereonetMeasurementType.None;

    [SerializeField] private Transform Pointer;
    [SerializeField] private LineRenderer _lineRenderer;
    
    [Header("UI")] 
    [SerializeField] GameObject _previewMeasurementUI;


    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.hoverEntered.AddListener(null);
    }

    private void Update()
    {
        var ray = new Ray(Pointer.position, Pointer.forward);
        _lineRenderer.SetPosition(1, Physics.Raycast(ray, out var hit) ? Pointer.InverseTransformPoint(hit.point) : Vector3.zero);
    }

    public void SetMeasurementType(StereonetMeasurementType type)
    {
        _stereonetMeasurementType = type;
    }
    public void ActivatePoleMeasurement()
    {
        _stereonetMeasurementType = StereonetMeasurementType.Pole;
    }
    public void ActivateLineMeasurement()
    {
        _stereonetMeasurementType = StereonetMeasurementType.Line;
    }
    public void ActivatePlaneMeasurement()
    {
        _stereonetMeasurementType = StereonetMeasurementType.Plane;
    }

    public void ClearMeasurementType()
    {
        _stereonetMeasurementType = StereonetMeasurementType.None;
    }

    public void ShowUI()
    {
        _lineRenderer.enabled = true;
        _previewMeasurementUI.SetActive(true);
        enabled = true;
    }

    public void HideUI()
    {
        _lineRenderer.enabled = false;
        _previewMeasurementUI.SetActive(false);
        enabled = false;
    }

    public void Measure()
    {
        if (_stereonetMeasurementType == StereonetMeasurementType.None)
        {
            return;
        }
        
        var ray = new Ray(Pointer.position, Pointer.forward);
        if (Physics.Raycast(ray, out var hit) && hit.transform.tag.Equals("Terrain"))
        {
            switch (_stereonetMeasurementType)
            {
                case StereonetMeasurementType.Pole:
                    PolePlotting.instance.UseTool(hit);
                    break;
                case StereonetMeasurementType.Line:
                    LinePlotting.instance.UseTool(hit);
                    break;
                case StereonetMeasurementType.Plane:
                    PlanePlotting.instance.UseTool(hit);
                    break;
                default:
                    break;
            }
        }
    }
}
