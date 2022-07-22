using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class StereonetMapMarker : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform Icon;
    [SerializeField] private TMP_Text IdText;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetPosition(Vector2 pos)
    {
        _rectTransform.localPosition = pos;
    }

    public void SetForward(Vector2 forward)
    {
        Icon.forward = new Vector3(forward.x, 0f, forward.y);
    }

    public void SetId(int id)
    {
        IdText.text = $"{id}";
    }

    public void SetFontSize(float fontSize)
    {
        IdText.fontSize = fontSize;
    }
}