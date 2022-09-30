using System.Collections;
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
    }

    public void SetRockModel(GameObject prefab, Quaternion orientation, Vector3 offset, float defaultDistAway)
    {
        if (_rockModel)
        {
            Destroy(_rockModel.gameObject);
        }

        _rockModel = Instantiate(prefab, Vector3.zero, orientation, _rockModelParent).transform;
        _rockModel.gameObject.SetLayerRecursively(_rockModelLayerMask);
        _rockModel.localPosition = -offset;

        _defaultZoomValue = defaultDistAway;
        _rockCamera.transform.localPosition = new Vector3(0f, 0f, -defaultDistAway);
        _rockCamera.Render();
    }

    public void RotateRockModel()
    {
        var delta = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * 10f;
        _rockModelParent.Rotate(Vector3.right, delta.x, Space.World);
        _rockModelParent.Rotate(Vector3.up, -delta.y);

        _rockCamera.Render();
    }


    public void Zoom(float amount, float minDist, float maxDist)
    {
        var pos = _rockCamera.transform.position;
        var newZOffset = Mathf.Clamp(pos.z + amount, -maxDist, -minDist);
        _rockCamera.transform.position = new Vector3(pos.x, pos.y, newZOffset);
        
        RenderCamera();
    }

    public void ResetCameraRotation()
    {
        
    }

    public void ResetZoom()
    {
        _rockCamera.transform.localPosition = new Vector3(0f, 0f, -_defaultZoomValue);
    }

    private void RenderCamera() => StartCoroutine(RenderCameraCo());
    private IEnumerator RenderCameraCo()
    {
        yield return null;
        _rockCamera.Render();
    }
}
