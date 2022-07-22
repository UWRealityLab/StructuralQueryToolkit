using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSnapTool : MonoBehaviour
{

    [SerializeField] private Transform _snapTransform;
    
    [SerializeField] private Vector3 _localSnapPos;
    private Quaternion _localSnapRot;

    [SerializeField] private bool _isSnapping = false;
    private Coroutine _snapCoroutine;

    private Transform _parentTrans;

    private void Awake()
    {
        SetSnapTransformToCurrent();
    }

    public void SetSnapTransformToCurrent()
    {
        if (_snapTransform != null)
        {
            _localSnapPos = _snapTransform.localPosition;
            _localSnapRot = _snapTransform.localRotation;
        }
        else
        {
            _parentTrans = transform.parent;
            _localSnapPos = transform.localPosition;
            _localSnapRot = transform.localRotation;
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
            transform.localPosition = _localSnapPos;
            transform.localRotation = _localSnapRot;
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
            transform.localPosition = Vector3.Lerp(transform.localPosition, _localSnapPos, 0.3f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _localSnapRot, 0.3f);
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = _localSnapPos;
        transform.localRotation = _localSnapRot;

        _isSnapping = false;
    }
    
}
