using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereonetCamera : MonoBehaviour
{
    public static StereonetCamera instance;

    public Camera cam;

    private float frameTime = 1 / 30f;
    private float timeLeft;

    private void Awake()
    {
        instance = this;
        timeLeft = frameTime;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            cam.Render();
            timeLeft = frameTime;
        }
    }

    private void OnDisable()
    {
        cam.Render();
    }

    public void Toggle()
    {
        cam.enabled = !cam.enabled;
    }

}