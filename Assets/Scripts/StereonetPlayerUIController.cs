using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StereonetPlayerUIController : MonoBehaviour
{
    public static StereonetPlayerUIController instance;
    
    [SerializeField] private Button stereonetButton;
    [SerializeField] private ToggleButtonsController toggleController;

    private void Awake()
    {
        instance = this;
    }

    public void HideStereonetUI()
    {
        return; // TODO
        
        if (toggleController.GetState())
        {
            stereonetButton.onClick.Invoke();
        }
    }
}
