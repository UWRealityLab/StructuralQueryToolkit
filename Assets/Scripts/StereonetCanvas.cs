using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereonetCanvas : MonoBehaviour
{
    public static StereonetCanvas Instance;
    public GameObject Stereonet2DContainer;

    private void Awake()
    {
        Instance = this;
    }
}
