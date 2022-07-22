using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRRayInteractor))]
public class UserInterfaceRay : MonoBehaviour
{
    private XRRayInteractor _rayInteractor;
    private LineRenderer _lineRenderer;

    void Awake()
    {
        _rayInteractor = GetComponent<XRRayInteractor>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var isHittingUI = _rayInteractor.TryGetCurrentUIRaycastResult(out var hit);

    }
}
