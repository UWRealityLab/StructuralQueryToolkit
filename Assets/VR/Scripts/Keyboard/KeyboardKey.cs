using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEditor;

[ExecuteInEditMode]
public class KeyboardKey : MonoBehaviour
{


    public bool isSpecialKey = false;
    //[DrawIf("isSpecialKey", true, DrawIfAttribute.DisablingType.DontDraw)]
    public UnityEvent pressEvent;

    private TextMeshProUGUI text;

    [DrawIf("isSpecialKey", false, DrawIfAttribute.DisablingType.DontDraw)]
    public bool hasUppercase = true;

    [DrawIf("isSpecialKey", false, DrawIfAttribute.DisablingType.DontDraw)]
    public string lowerCaseKey;

    [DrawIf("isSpecialKey", false, DrawIfAttribute.DisablingType.DontDraw)]
    public string upperCaseKey;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {

    #if UNITY_EDITOR
/*        if (!isSpecialKey) {
            text = GetComponentInChildren<TextMeshProUGUI>();
            text.text = transform.gameObject.name.ToLower();
            lowerCaseKey = transform.gameObject.name.ToLower();
            upperCaseKey = transform.gameObject.name.ToUpper();
        }*/
#endif
    }

    public void SetText(string s) {
        text.text = s;
    }

    public void WriteToText() {
        KeyboardController.instance.Write(text.text);
    }

    public void Hover() {
        animator.SetBool("isToggled", true);
    }

    public void EndHover() {
        animator.SetBool("isToggled", false);
    }

}

