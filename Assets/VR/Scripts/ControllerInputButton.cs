using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInputButton : MonoBehaviour
{
    public InputActionProperty action;

    public UnityEvent OnPress;
    public UnityEvent OnRelease;

    private void Awake()
    {
        action.action.performed += _ => OnPress.Invoke();
        action.action.canceled += _ => OnRelease.Invoke();
    }

    private void OnDestroy()
    {
        action.action.performed -= OnPressed;
        action.action.canceled -= OnReleased;
    }

    private void OnEnable()
    {
        action.action.Enable();
    }

    private void OnDisable()
    {
        action.action.Disable();
    }

    private void OnReleased(InputAction.CallbackContext context)
    {
        OnRelease.Invoke();
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        OnPress.Invoke();
    }
}
