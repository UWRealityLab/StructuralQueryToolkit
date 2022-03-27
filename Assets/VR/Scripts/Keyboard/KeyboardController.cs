using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.IntegralTransforms;
using UnityEngine;
using TMPro;
using UnityEditor;

public class KeyboardController : MonoBehaviour
{
    public static KeyboardController instance;

    [SerializeField] private TMP_InputField inputBox;

    [SerializeField] private Animator animator;
    private bool isUpperCase = false;
    private KeyboardKey[] keys;

    private bool _isToggled = false;
    private static readonly int _isToggledAnimParam = Animator.StringToHash("isToggled");

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        keys = GetComponentsInChildren<KeyboardKey>();

        // Naive way of getting every input field to the keyboard
        var inputFields = FindObjectsOfType<TMP_InputField>(true);
        foreach (var inputField in inputFields)
        {
            inputField.onSelect.AddListener(str =>
            {
                inputBox = inputField;
                OpenKeyboard();
            });
        }
        
        StereonetDashboard.instance.OnAddStereonetCard.AddListener(card =>
        {
            card.titleInputField.onSelect.AddListener(str =>
            {
                inputBox = card.titleInputField;
                OpenKeyboard();
            });
        });

        StereonetFullscreenManager.Instance.OnAddMeasurementGroup.AddListener(measurementsGroup =>
        {
            measurementsGroup.groupNameText.onSelect.AddListener(str =>
            {
                inputBox = measurementsGroup.groupNameText;
                OpenKeyboard();
            });
        });

        gameObject.SetActive(false);

    }

    public void Write(KeyboardKey key)
    {
        if (!key.isSpecialKey)
        {
            inputBox.text += isUpperCase && key.hasUppercase ? key.upperCaseKey : key.lowerCaseKey;
        }

        if (isUpperCase)
        {
            ToggleCase();
        }
    }

    public void Write(string str)
    {
        inputBox.text += str;
        
        if (isUpperCase)
        {
            ToggleCase();
        }
    }

    public void MoveLeft() {

    }

    public void MoveRight() {
        
    }

    public void Backspace() {
        inputBox.text = inputBox.text.Substring(0, inputBox.text.Length - 1);
    }

    public void Enter() {
        inputBox.DeactivateInputField();
        inputBox = null;
        CloseKeyboard();
    }

    public void OpenKeyboard()
    {
        gameObject.SetActive(true);
        _isToggled = true;
        animator.SetBool(_isToggledAnimParam, true);
    }
    
    public void CloseKeyboard()
    {
        _isToggled = false;
        animator.SetBool(_isToggledAnimParam, false);
    }

    public void ToggleCase() 
    {
        isUpperCase = !isUpperCase;
        foreach (var key in keys) {
            if (isUpperCase)
            {
                key.ToUpperCase();
            }
            else
            {
                key.ToLowerCase();
            }
        }
    }
    
    public void ToggleKeyboard()
    {
        _isToggled = !_isToggled;
        if (_isToggled)
        {
            OpenKeyboard();
        }
        else
        {
            CloseKeyboard();
        }
    }
}
