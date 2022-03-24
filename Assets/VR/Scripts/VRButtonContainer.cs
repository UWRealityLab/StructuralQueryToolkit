using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VRButtonContainer : MonoBehaviour
{
    private VRButtonUI[] _buttons;

    private VRButtonUI _activeButton;

    private void Awake()
    {
        _buttons = GetComponentsInChildren<VRButtonUI>(true);
    }

    private void Start()
    {
        foreach (var vrButton in _buttons)
        {
            vrButton.Button.onClick.AddListener(() =>
            {
                UpdateActiveButton(vrButton);
            });
        }
    }

    private void UpdateActiveButton(VRButtonUI button)
    {
        if (_activeButton == button)
        {
            _activeButton.Deselect();
            _activeButton = null;
            return;
        }
        
        if (_activeButton)
        {
            _activeButton.Deselect();
        }

        _activeButton = button;
        _activeButton.Select();
    }
}
