using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;

    public static TooltipSystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<TooltipSystem>();
            }
            return instance; 
        }
    }
    
    public float HoverTime = 1f;

    [SerializeField] private TooltipUI tooltipUI;
    
    private Canvas _canvas;

    private void Awake()
    {
        instance = this;
        _canvas = GetComponent<Canvas>();
    }
    
    public void Show(Vector2 position, Vector2 offset, string header = "", string description = "")
    {
        tooltipUI.gameObject.SetActive(true);
        tooltipUI.SetText(header, description, position + (offset * _canvas.renderingDisplaySize));
    }

    public void Hide()
    {
        tooltipUI.gameObject.SetActive(false);
    }
    
}