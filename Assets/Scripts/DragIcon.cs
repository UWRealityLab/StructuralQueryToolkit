using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragIcon : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform WindowTransform;

    private Rect canvasRect;
    private Rect rectTransform;

    private void Start()
    {
        rectTransform = WindowTransform.GetComponent<RectTransform>().rect;
        canvasRect = canvas.GetComponent<RectTransform>().rect;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var anchoredPosition = WindowTransform.anchoredPosition;
        var pivot = WindowTransform.pivot;
        var anchorMin = WindowTransform.anchorMin;
        var anchorMax = WindowTransform.anchorMax;
        var width = rectTransform.width;
        var height = rectTransform.height;
        
        anchoredPosition += eventData.delta / canvas.scaleFactor;
        WindowTransform.anchoredPosition = anchoredPosition;
        
        WindowTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(anchoredPosition.x, -(canvasRect.width * anchorMin.x) + (width * pivot.x), (canvasRect.width * (1 - anchorMin.x) - (width * (1 - pivot.x)))), 
            Mathf.Clamp(anchoredPosition.y, 0 + (height * pivot.y), canvasRect.height - (height * (1 - pivot.y))));
        
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Cursor.visible = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Cursor.visible = true;
    }
}
