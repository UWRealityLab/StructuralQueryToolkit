using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class FreeMoveLocomotion : MonoBehaviour
{
    public InputActionProperty LeftHandFreeMoveAction;
    public InputActionProperty RightHandFreeMoveAction;

    [SerializeField] private Volume _postProcessingVolume;

    private bool _isGrounded = true;
    
    private void Awake()
    {
        //LeftHandFreeMoveAction.action.ad
    }

    private void OnEnable()
    {
        LeftHandFreeMoveAction.EnableDirectAction();
        RightHandFreeMoveAction.EnableDirectAction();
    }

    private void OnDisable()
    {
        LeftHandFreeMoveAction.DisableDirectAction();
        RightHandFreeMoveAction.DisableDirectAction();
    }

    private bool CanMove(Vector3 dir, float dist)
    {
        Assert.IsTrue(dist > 0);
        return Physics.SphereCast(transform.position, 0.5f, dir.normalized, out var hit, dist);
    }

}
