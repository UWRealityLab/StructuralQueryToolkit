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
        Ruler,
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

    private void Start()
    {
        _previewMeasurementUI.SetActive(false);
    }

    private void Update()
    {
        var ray = new Ray(Pointer.position, Pointer.forward);
        var isHit = Physics.Raycast(ray, out var hit);
        _lineRenderer.SetPosition(1, isHit ? Pointer.InverseTransformPoint(hit.point) : Vector3.zero);
    }

    public void SetMeasurementType(StereonetMeasurementType type)
    {
        _stereonetMeasurementType = type;
    }
    public void ActivatePoleMeasurement()
    {
        if (_stereonetMeasurementType == StereonetMeasurementType.Pole)
        {
            ClearMeasurementType();
        }
        _stereonetMeasurementType = StereonetMeasurementType.Pole;
    }
    public void ActivateLineMeasurement()
    {
        if (_stereonetMeasurementType == StereonetMeasurementType.Line)
        {
            ClearMeasurementType();
        }

        _stereonetMeasurementType = StereonetMeasurementType.Line;
    }
    public void ActivatePlaneMeasurement()
    {
        if (_stereonetMeasurementType == StereonetMeasurementType.Plane)
        {
            ClearMeasurementType();
        }

        _stereonetMeasurementType = StereonetMeasurementType.Plane;
    }
    public void ActivateRulerPlotting()
    {
        if (_stereonetMeasurementType == StereonetMeasurementType.Ruler)
        {
            ClearMeasurementType();
        }

        _stereonetMeasurementType = StereonetMeasurementType.Ruler;
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
                case StereonetMeasurementType.Ruler:
                    RulerPlotting.instance.UseTool(hit);
                    break;
                default:
                    break;
            }
        }
    }
}
