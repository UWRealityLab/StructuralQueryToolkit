using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class LocomotionManager : MonoBehaviour
{
    private enum LocomotionType
    {
        Teleport,
        FreeMove,
        None
    }

    [SerializeField] private LocomotionType _leftHandLocomotionType;
    [SerializeField] private LocomotionType _rightHandLocomotionType;

    [Header("Popup UI")] 
    public float DelayToFadeTime;
    public float FadeTime;
    [SerializeField] private CanvasGroup _leftLocomotionPopupCanvasGroup;
    [SerializeField] private CanvasGroup _rightLocomotionPopupCanvasGroup;
    [SerializeField] private TMP_Text _leftPopupText;
    [SerializeField] private TMP_Text _rightPopupText;

    private Coroutine _leftPopupCo;
    private Coroutine _rightPopupCo;
    
    [Header("Misc")]
    public InputActionProperty LeftHandSwitchLocomotionAction;
    public InputActionProperty RightHandSwitchLocomotionAction;

    public TeleportRay _leftHandTeleport;
    public FreeMoveLocomotion _leftHandFreeMove;

    public TeleportRay _rightHandTeleport;
    public FreeMoveLocomotion _rightHandFreeMove;
    
    private void Awake()
    {
        LeftHandSwitchLocomotionAction.action.performed += _ =>
        {
            _leftHandLocomotionType = _leftHandLocomotionType == LocomotionType.Teleport ? LocomotionType.FreeMove : LocomotionType.Teleport;
            var isTeleport = _leftHandLocomotionType == LocomotionType.Teleport;
            _leftHandTeleport.SetState(isTeleport);
            _leftHandFreeMove.enabled = !isTeleport;

            if (_leftPopupCo != null)
            {
                StopCoroutine(_leftPopupCo);
            }
            _leftPopupCo = StartCoroutine(ShowLocomotionPopup(_leftHandLocomotionType, _leftLocomotionPopupCanvasGroup, _leftPopupText));
        };
        
        RightHandSwitchLocomotionAction.action.performed += _ =>
        {
            _rightHandLocomotionType = _rightHandLocomotionType == LocomotionType.Teleport ? LocomotionType.FreeMove : LocomotionType.Teleport;
            var isTeleport = _rightHandLocomotionType == LocomotionType.Teleport;
            _rightHandTeleport.SetState(isTeleport);
            _rightHandFreeMove.enabled = !isTeleport;
            
            if (_rightPopupCo != null)
            {
                StopCoroutine(_rightPopupCo);
            }
            _rightPopupCo = StartCoroutine(ShowLocomotionPopup(_rightHandLocomotionType, _rightLocomotionPopupCanvasGroup, _rightPopupText));
        };
    }

    private void OnEnable()
    {
        LeftHandSwitchLocomotionAction.EnableDirectAction();
        RightHandSwitchLocomotionAction.EnableDirectAction();
    }

    private void OnDisable()
    {
        LeftHandSwitchLocomotionAction.DisableDirectAction();
        RightHandSwitchLocomotionAction.DisableDirectAction();
    }
    
    private IEnumerator ShowLocomotionPopup(LocomotionType locomotionType, CanvasGroup window, TMP_Text txt)
    {
        switch (locomotionType)
        {
            case LocomotionType.Teleport:
                txt.text = "Teleport";
                break;
            case LocomotionType.FreeMove:
                txt.text = "Free Move";
                break;
        }
        window.alpha = 1f;

        var elapsedTime = 0f;
        while (elapsedTime < DelayToFadeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            elapsedTime += Time.deltaTime;
            window.alpha = 1f - (elapsedTime / FadeTime);
            yield return null;
        }

        window.alpha = 0f;

    }
}
