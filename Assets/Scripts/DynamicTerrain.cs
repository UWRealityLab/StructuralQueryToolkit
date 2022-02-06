using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), DisallowMultipleComponent]
public class DynamicTerrain : MonoBehaviour
{
    private Flag[] _flags;

    private void Awake()
    {
        _flags = Array.Empty<Flag>();
    }

    public void UpdateStereonet()
    {
        foreach (var measurement in _flags)
        {
            measurement.UpdateStereonet();
        }
    }

    private void OnValidate()
    {
        transform.tag = "Terrain";
    }

    private void OnTransformChildrenChanged()
    {
        _flags = GetComponentsInChildren<Flag>(true);
    }
}
