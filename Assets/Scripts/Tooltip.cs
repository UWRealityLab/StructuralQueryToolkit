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

    private Coroutine _showTooltipCo;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _showTooltipCo = StartCoroutine(ShowTooltipCoroutine());
    }

    private IEnumerator ShowTooltipCoroutine()
    {
        var elapsedTime = 0f;
        while (elapsedTime < TooltipSystem.Instance.HoverTime)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _showTooltipCo = null;
        TooltipSystem.Instance.Show(new Vector2(transform.position.x, transform.position.y), Offset, HeaderText, DescriptionText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_showTooltipCo != null)
        {
            StopCoroutine(_showTooltipCo);
            _showTooltipCo = null;
        }
        TooltipSystem.Instance.Hide();
    }
}