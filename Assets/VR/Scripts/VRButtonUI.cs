using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class VRButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image BackgroundImg;
    [HideInInspector] public Button Button;
    
    [SerializeField] private Color32 _selectedColor;
    //[SerializeField] private float _hoverDarken;
    [SerializeField] private Color32 _unselectedColor;

    public UnityEvent OnSelect;
    public UnityEvent OnDeselect;

    [SerializeField] private bool isSelected = false; 
    
    private void Awake()
    {
        Button = GetComponent<Button>();
    }
    
    public void Select()
    {
        isSelected = true;
        BackgroundImg.color = _selectedColor;
        OnSelect.Invoke();
    }
    
    public void Deselect()
    {
        isSelected = false;
        BackgroundImg.color = _unselectedColor;
        OnDeselect.Invoke();
    }

    public void Toggle()
    {
        isSelected = !isSelected;
        
        if (isSelected)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
