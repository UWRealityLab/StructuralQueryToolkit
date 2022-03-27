using System;
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

    private Transform _parentTrans;

    private void Awake()
    {
        SetSnapTransformToCurrent();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            _parentTrans = transform.parent;
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

    private void OnEnable()
    {
        if (_snapTransform == null)
        {
            transform.parent = _parentTrans;
            transform.localPosition = _snapPos;
            transform.localRotation = _snapRot;
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
