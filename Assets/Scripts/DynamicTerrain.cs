using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTerrain : MonoBehaviour
{
    private void Update()
    {
        UpdateStereonet();
    }

    public void UpdateStereonet()
    {
        var children = GetComponentsInChildren<Flag>(true);

        foreach (var measurement in children)
        {
            measurement.UpdateStereonet();
        }
    }
}
