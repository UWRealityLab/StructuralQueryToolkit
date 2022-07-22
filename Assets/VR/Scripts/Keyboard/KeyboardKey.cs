using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteInEditMode]
public class KeyboardKey : XRSimpleInteractable, IPointerDownHandler
{
    public bool isSpecialKey = false;
    //[DrawIf("isSpecialKey", true, DrawIfAttribute.DisablingType.DontDraw)]
    public UnityEvent pressEvent;

    private Button _button;

    private TextMeshProUGUI text;

    [DrawIf("isSpecialKey", false, DrawIfAttribute.DisablingType.DontDraw)]
    public bool hasUppercase = true;

    [DrawIf("isSpecialKey", false, DrawIfAttribute.DisablingType.DontDraw)]
    public string lowerCaseKey;

    [DrawIf("isSpecialKey", false, DrawIfAttribute.DisablingType.DontDraw)]
    public string upperCaseKey;

    private Animator animator;
    private static readonly int _isHovered = Animator.StringToHash("isHovered");

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        if (!isSpecialKey)
        {
            text ??= GetComponentInChildren<TextMeshProUGUI>();
            lowerCaseKey = gameObject.name;
            upperCaseKey = lowerCaseKey.ToUpper();
            text.text = lowerCaseKey;
        }

    }

    private void OnValidate()
    {
        animator ??= GetComponent<Animator>();

        if (!isSpecialKey)
        {
            text ??= GetComponentInChildren<TextMeshProUGUI>();
            lowerCaseKey = gameObject.name;
            upperCaseKey = lowerCaseKey.ToUpper();
            text.text = lowerCaseKey;
        }
    }

    public void ToLowerCase()
    {
        if (!isSpecialKey && text != null)
        {
            text.text = lowerCaseKey;
        }
    }

    public void ToUpperCase()
    {
        if (!isSpecialKey && hasUppercase)
        {
            text.text = upperCaseKey;
        }
    }

    public void WriteToText() {
        KeyboardController.instance.Write(this);
    }
    
    public void WriteToText(string str)
    {
        KeyboardController.instance.Write(str);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
    }


    public void Hover() {
        animator.SetBool(_isHovered, true);
    }

    public void EndHover() {
        animator.SetBool(_isHovered, false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressEvent.Invoke();

        if (!isSpecialKey)
        {
            KeyboardController.instance.Write(this);
        }
    }
}

