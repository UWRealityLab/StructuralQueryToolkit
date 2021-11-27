using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorSwatchButton : MonoBehaviour
{
    public Color color;

    private void Start()
    {
        color = GetComponent<Image>().color;
    }

    private void OnValidate()
    {
        color = GetComponent<Image>().color;
    }
}
