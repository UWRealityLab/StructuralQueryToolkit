using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ProceedButtonTrigger : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent clickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickEvent.Invoke();
        Destroy(this);
    }

    public void Destroy()
    {
        Destroy(this);
    }
}
