using System;
using System.Collections;
using System.Collections.Generic;
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
        WindowTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        
        WindowTransform.anchoredPosition = new Vector2(Mathf.Clamp(WindowTransform.anchoredPosition.x, 0, canvasRect.width - rectTransform.width), Mathf.Clamp(WindowTransform.anchoredPosition.y, 0, canvasRect.height - rectTransform.height));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Cursor.visible = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.visible = true;
    }
}
