using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Animator))]
public class FloatingActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Animator animator;

    public bool hasText = false;

    [DrawIf("hasText", true, DrawIfAttribute.DisablingType.DontDraw)]
    public GameObject buttonText;

    public UnityEvent enterEvent;
    public UnityEvent exitEvent;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            return;
        }
        animator.SetBool("Highlighted", true);
        if (hasText)
        {
            //buttonText.SetActive(true);
        }
        enterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Highlighted", false);
        if (hasText)
        {
            //buttonText.SetActive(false);
        }
        exitEvent.Invoke();
    }

    private void OnDisable()
    {
        // For certain cases when object is disabled and re-enabled (ex: entering and leaving the map mode)
        // it's very likely the mouse is not over the the floating button anymore
        animator.SetBool("Highlighted", false);
    }
}
