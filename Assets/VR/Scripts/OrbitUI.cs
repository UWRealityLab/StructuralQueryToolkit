﻿using System;
using System.Collections;
using UnityEngine;

public class OrbitUI : MonoBehaviour
{
    public Transform PlayerTrans;
    
    [Header("Vertical Offset")]
    [SerializeField] private float _maxVerticalDistanceFromCamera = 0.5f;
    [SerializeField] private float _verticalOffset = 0.2f;
    
    [Header("Distance away from the target")]
    public float Offset;

    [Range(0f, 180f)] 
    public float maxRotation = 25f;

    private bool isTransitioning = false;

    public bool useDifferentCenterTransform;
    [DrawIf("useDifferentCenterTransform", true, DrawIfAttribute.DisablingType.ReadOnly)]
    public Transform centerTransform;

    private Vector3 targetPos;
    private Vector2 _latestForwardDir; // The latest forward direction where the orbiting UI moved towards


    private Coroutine _correctingYPositionCo;

    private void Awake()
    {
        var playerForward = new Vector3(PlayerTrans.forward.x, 0f, PlayerTrans.forward.z).normalized;
        _latestForwardDir = new Vector2(playerForward.x, playerForward.z).normalized;
        transform.position = PlayerTrans.position + playerForward * Offset;
    }

    public void SetCenterTransform(bool isEnable) {
        useDifferentCenterTransform = isEnable;
    }

    private void LateUpdate() 
    {
        if (isTransitioning) {
            return;
        }
        
        var playerForward = new Vector2(PlayerTrans.forward.x, PlayerTrans.forward.z).normalized;
        var degreesFromUI = Vector2.Angle(playerForward, _latestForwardDir);
        
        //print(degreesFromUI);
        
        if (degreesFromUI > maxRotation)
        {
            _latestForwardDir = playerForward;
            StartCoroutine(LerpToTargetPosition());
        }

        AdjustVerticalOffset();
    }

    private IEnumerator LerpToTargetPosition() 
    {
        isTransitioning = true;
        
        // TODO switch to local position 
        
        var forward = new Vector3(_latestForwardDir.x, 0f, _latestForwardDir.y).normalized;
        targetPos = PlayerTrans.position + forward * Offset;
        targetPos.y = transform.position.y;
        
        float timeLeft = 0.25f;
        while (timeLeft > 0f) {
            timeLeft -= Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, targetPos, 0.3f);
            transform.LookAt(PlayerTrans.position);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y , 0f);
            transform.forward = -transform.forward;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);

            yield return null;
        }
        transform.position = targetPos;
        transform.LookAt(PlayerTrans.position);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        transform.forward = -transform.forward;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);

        isTransitioning = false;
    }

    public void Toggle() 
    {
        this.enabled = !this.enabled;
    }

    private void AdjustVerticalOffset()
    {
        var verticalDistApart = Mathf.Abs(PlayerTrans.position.y + _verticalOffset - transform.position.y);
        var isExceedingVerticalOffset = verticalDistApart > _maxVerticalDistanceFromCamera;
        
        if (isExceedingVerticalOffset && _correctingYPositionCo == null)
        {
            _correctingYPositionCo = StartCoroutine(CorrectVerticalOffsetCo());
        }

        IEnumerator CorrectVerticalOffsetCo()
        {
            var newYPos = new Vector3(transform.position.x, PlayerTrans.position.y + _verticalOffset, transform.position.z);;
            
            float timeLeft = 0.25f;
            while (timeLeft > 0f) 
            {
                timeLeft -= Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, newYPos, 0.3f);
                yield return null;
            }

            transform.position = newYPos;

            _correctingYPositionCo = null;
        }
    }
}
