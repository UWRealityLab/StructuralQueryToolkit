using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StereonetDragController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    
    private void Update()
    {
        if (!_isDragging)
        {
            return;
        }

        StereonetsController.instance.RotateStereonet();
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
