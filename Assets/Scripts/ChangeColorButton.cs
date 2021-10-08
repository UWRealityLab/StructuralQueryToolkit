using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ChangeColorButton : MonoBehaviour
{
    Image image;

    [HideInInspector] public bool isEnabled;

    [SerializeField] Color BackgroundOnColor;
    [SerializeField] Color BackgroundOffColor;

    public bool hasSubImage = false;
    [DrawIf("hasSubImage", true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    Image subImage;
    [DrawIf("hasSubImage", true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    Color subImageOnColor;
    [DrawIf("hasSubImage", true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    Color subImageOffColor;

    public bool hasText = false;
    [DrawIf("hasText", true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    TextMeshProUGUI buttonText;
    [DrawIf("hasText", true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    Color buttonTextOnColor;
    [DrawIf("hasText", true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    Color buttonTextOffColor;

    public UnityEvent enableEvent;
    public UnityEvent disableEvent;

    private Animator animator;

    private void Awake()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    public void SetButtonState(bool state)
    {
        isEnabled = state;
        if (isEnabled)
        {
            image.color = BackgroundOnColor;
            if (hasSubImage)
            {
                subImage.color = subImageOnColor;
            }
            if (hasText)
            {
                buttonText.color = buttonTextOnColor;
            }
            enableEvent.Invoke();
        }
        else
        {
            image.color = BackgroundOffColor;
            if (hasSubImage)
            {
                subImage.color = subImageOffColor;
            }
            if (hasText)
            {
                buttonText.color = buttonTextOffColor;
            }
            disableEvent.Invoke();
        }
    }

    public void SetColorsOnAnimatorParameter(string parameter)
    {
        SetButtonState(animator.GetBool(parameter));
    }

    public void ToggleState()
    {
        SetButtonState(!isEnabled);


    }
}
