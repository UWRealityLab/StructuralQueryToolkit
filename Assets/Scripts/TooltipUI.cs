using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TooltipUI : MonoBehaviour
{
    
    private RectTransform _RectTransform;
    [SerializeField] private TextMeshProUGUI HeaderText;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private LayoutElement tooltipLayoutElement;

    public int CharacterWrapLimit = 50;

    private void Awake()
    {
        _RectTransform = GetComponent<RectTransform>();
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
        _RectTransform.position = position;
    }

    private void UpdateUISize()
    {
        var headerLength = HeaderText.text.Length;
        var descLength = DescriptionText.text.Length;
        tooltipLayoutElement.enabled = headerLength > CharacterWrapLimit || descLength > CharacterWrapLimit ? true : false;
    }
}
