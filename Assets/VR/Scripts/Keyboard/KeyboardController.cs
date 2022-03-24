using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class KeyboardController : MonoBehaviour
{
    public static KeyboardController instance;

    public TMP_InputField inputBox;

    private Animator animator;
    private bool isUpperCase = false;
    private KeyboardKey[] keys;

    public KeyboardHand leftHand;
    public KeyboardHand rightHand;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();
        keys = GetComponentsInChildren<KeyboardKey>();
    }

    /*private void OnEnable() {
        leftHand.enabled = true;
        rightHand.enabled = true;
    }

    private void OnDisable() {
        leftHand.enabled = false;
        rightHand.enabled = false;
    }*/

    public void Write(string s) {
        inputBox.text += s;
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

    public void CloseKeyboard() {
        animator.SetBool("isToggled", false);
        leftHand.enabled = false;
        rightHand.enabled = false;
    }

    public void ToggleCase() {

        foreach (var key in keys) {
            if (!key.isSpecialKey && key.hasUppercase) {
                if (isUpperCase) {
                    key.SetText(key.lowerCaseKey);
                } else {
                    key.SetText(key.upperCaseKey);
                }
            }
        }
        isUpperCase = !isUpperCase;
    }
}
