using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlphaButtonClickMask : MonoBehaviour 
{
    protected Image _image;

    public void Awake()
    {
        _image = GetComponent<Image>();

        _image.alphaHitTestMinimumThreshold = 1f;

    }
}