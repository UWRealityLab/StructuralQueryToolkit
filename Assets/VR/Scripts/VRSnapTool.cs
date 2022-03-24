using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSnapTool : MonoBehaviour
{

    [SerializeField] private Transform _snapTransform;
    
    private Vector3 _snapPos;
    private Quaternion _snapRot;

    private bool _isSnapping = false;
    private Coroutine _snapCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        SetSnapTransformToCurrent();
    }

    public void SetSnapTransformToCurrent()
    {
        if (_snapTransform != null)
        {
            _snapPos = _snapTransform.localPosition;
            _snapRot = _snapTransform.localRotation;
        }
        else
        {
            _snapPos = transform.localPosition;
            _snapRot = transform.localRotation;
        }
    }

    public void SnapBack()
    {
        if (!_isSnapping)
        {
            _snapCoroutine = StartCoroutine(SnapCoroutine());
        }
    }

    public void Deactivate()
    {
        if (_snapCoroutine != null)
        {
            StopCoroutine(_snapCoroutine);
            _snapCoroutine = null;
        }
        _isSnapping = false;
    }

    private IEnumerator SnapCoroutine()
    {
        _isSnapping = true;
        
        var timeLeft = 0.5f;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _snapPos, 0.3f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _snapRot, 0.3f);
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = _snapPos;
        transform.localRotation = _snapRot;

        _isSnapping = false;
    }
    
}
