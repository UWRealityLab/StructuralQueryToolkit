using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonsDrawer : MonoBehaviour
{
    [SerializeField] private ButtonDrawerButton activeButton;
    
    public void SelectButton(ButtonDrawerButton button)
    {
        activeButton.Deselect();
        activeButton = button;
        button.Select();
    }

    public Color GetActiveButtonColor()
    {
        return activeButton.GetColor();
    }
    
}
