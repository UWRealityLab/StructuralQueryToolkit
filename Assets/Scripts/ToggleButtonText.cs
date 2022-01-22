using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToggleButtonText : MonoBehaviour
{
    [SerializeField] private bool isToggled = false;

    [SerializeField] private string untoggledText;
    [SerializeField] private string toggledText;
    
    [SerializeField] private TextMeshProUGUI text;

    public void Toggle()
    {
        isToggled = !isToggled;
        text.text = isToggled ? toggledText : untoggledText;
    }

    public void SetState(bool state)
    {
        isToggled = state;
        text.text = isToggled ? toggledText : untoggledText;
    }

    public bool GetState()
    {
        return isToggled;
    }

}
