using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Management;

public class StereonetCamera : MonoBehaviour
{
    public static StereonetCamera instance;

    public Camera cam;

    private void Awake()
    {
        instance = this;
    }

    private void OnDisable()
    {
        Render();
    }

    public void Toggle()
    {
        cam.enabled = !cam.enabled;
    }

    public void UpdateStereonet()
    {
        StartCoroutine(UpdateStereonetCoroutine());
    }

    public void UpdateStereonetImmediate()
    {
        Render();
    }

    private IEnumerator UpdateStereonetCoroutine()
    {
        yield return new WaitForEndOfFrame();

        Render();
    }

    private void Render()
    {
        if (!GameController.instance.IsVR)
        {
            cam.Render();
        }
    }

}