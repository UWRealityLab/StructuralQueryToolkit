using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereonetCameraManagerVR : MonoBehaviour
{
    public static StereonetCameraManagerVR instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnDisable()
    {
        UpdateStereonetImmediate();
    }
    
    public void UpdateStereonetImmediate()
    {
        StereonetsController.instance.currStereonet.RenderCamera();
    }

    public void UpdateStereonet()
    {
        StartCoroutine(UpdateStereonetCoroutine());
    }

    private IEnumerator UpdateStereonetCoroutine()
    {
        yield return new WaitForEndOfFrame();
        StereonetsController.instance.currStereonet.RenderCamera();
    }
}
