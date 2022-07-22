using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RockModelDragController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    
    private void Update()
    {
        if (!_isDragging)
        {
            return;
        }

        RockPopupManager.Instance.RotateRockModel();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }
}
