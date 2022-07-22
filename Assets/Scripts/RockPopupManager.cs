using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Fbx;
using Cinemachine;
using Unity.XR.CoreUtils;
using UnityEngine;

public class RockPopupManager : MonoBehaviour
{
    public static RockPopupManager Instance;
    
    [SerializeField] private Camera _rockCamera;
    [SerializeField] private Transform _rockModelParent;

        
    private Transform _rockModel;
    private float _defaultZoomValue;
    private int _rockModelLayerMask;
    
    private void Awake()
    {
        Instance = this;
        _rockModelLayerMask = LayerMask.NameToLayer("Rock Models");
        _defaultZoomValue = _rockCamera.orthographicSize;
    }

    public void SetRockModel(GameObject prefab, Quaternion orientation, Vector3 offset)
    {
        if (_rockModel)
        {
            Destroy(_rockModel.gameObject);
        }

        _rockModel = Instantiate(prefab, Vector3.zero, orientation, _rockModelParent).transform;
        _rockModel.gameObject.SetLayerRecursively(_rockModelLayerMask);
        _rockModel.localPosition = -offset;
        
        _rockCamera.Render();
    }

    public void RotateRockModel()
    {
        var delta = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * 10f;
        _rockModelParent.Rotate(Vector3.right, delta.x, Space.World);
        _rockModelParent.Rotate(Vector3.up, -delta.y);

        _rockCamera.Render();
    }


    public void Zoom(float amount)
    {
        //_rockCamera.focalLength = Mathf.Clamp(_rockCamera.focalLength + amount, 2f, 200f);
        //_orbitalTransposer.m_FollowOffset.z = Mathf.Clamp(_orbitalTransposer.m_FollowOffset.z + amount, -100, -1.5f);
        var pos = _rockCamera.transform.position;
        var newZOffset = Mathf.Clamp(pos.z + amount, -10, 1.5f);
        _rockCamera.transform.position = new Vector3(pos.x, pos.y, newZOffset);
        
        RenderCamera();
    }

    public void ResetCameraRotation()
    {
        
    }

    public void ResetZoom()
    {
        _rockCamera.orthographicSize = _defaultZoomValue;
    }

    private void RenderCamera() => StartCoroutine(RenderCameraCo());
    private IEnumerator RenderCameraCo()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _rockCamera.Render();
    }
}
