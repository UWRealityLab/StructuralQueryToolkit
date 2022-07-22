using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    
    private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI HeaderText;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private LayoutElement tooltipLayoutElement;

    public int CharacterWrapLimit = 50;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    public void SetText(string header, string description, Vector2 position)
    {
        if (string.IsNullOrEmpty(header))
        {
            HeaderText.gameObject.SetActive(false);
        }
        else
        {
            HeaderText.gameObject.SetActive(true);
            HeaderText.text = header;
        }
        
        if (string.IsNullOrEmpty(description))
        {
            DescriptionText.gameObject.SetActive(false);
        }
        else
        {
            DescriptionText.gameObject.SetActive(true);
            DescriptionText.text = description;
        }
        
        UpdateUISize();
        _rectTransform.position = position;
    }

    private void UpdateUISize()
    {
        var headerLength = HeaderText.text.Length;
        var descLength = DescriptionText.text.Length;
        tooltipLayoutElement.enabled = headerLength > CharacterWrapLimit || descLength > CharacterWrapLimit ? true : false;
    }
}