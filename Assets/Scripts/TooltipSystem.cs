using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem instance;
    public float HoverTime = 1f;

    [SerializeField] private TooltipUI tooltipUI;
    
    private static Canvas _canvas;

    private void Awake()
    {
        instance = this;
        _canvas = GetComponent<Canvas>();
    }


    public static void Show(Vector2 position, Vector2 offset, string header = "", string description = "")
    {
        if (!instance)
        {
            return;
        }
        
        instance.tooltipUI.gameObject.SetActive(true);
        instance.tooltipUI.SetText(header, description, position + (offset * _canvas.renderingDisplaySize));
    }

    public static void Hide()
    {
        if (!instance)
        {
            return;
        }

        instance.tooltipUI.gameObject.SetActive(false);
    }
    
}
