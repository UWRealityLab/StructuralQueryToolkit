using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    
    
    [TextArea]
    public string HeaderText;
    [TextArea(4, 32)]
    public string DescriptionText;
    public Vector2 Offset;

    private Coroutine _ShowTooltipCo;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _ShowTooltipCo = StartCoroutine(ShowTooltipCoroutine());
    }

    private IEnumerator ShowTooltipCoroutine()
    {
        var elapsedTime = 0f;
        while (elapsedTime < TooltipSystem.instance.HoverTime)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _ShowTooltipCo = null;
        TooltipSystem.Show(new Vector2(transform.position.x, transform.position.y), Offset, HeaderText, DescriptionText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_ShowTooltipCo != null)
        {
            StopCoroutine(_ShowTooltipCo);
            _ShowTooltipCo = null;
        }
        TooltipSystem.Hide();
    }
}
