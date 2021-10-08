using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public MeshRenderer flagMeshRenderer;
    public Stereonet stereonet;
    public Transform stereonetPoint;

    /// <summary>
    /// If flags are moved dynamically, then their poles need to be updated
    /// </summary>
    public void UpdateStereonet()
    {
        stereonet.ChangePoleData(transform.up, stereonetPoint);
    }
}
