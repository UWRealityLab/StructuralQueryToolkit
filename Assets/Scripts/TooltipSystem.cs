using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem instance;
    public float HoverTime = 1f;

    [SerializeField] private TooltipUI tooltipUI;
    
    private void Awake()
    {
        instance = this;
    }


    public static void Show(Vector2 position, string header = "", string description = "")
    {
        if (!instance)
        {
            return;
        }
        instance.tooltipUI.gameObject.SetActive(true);
        instance.tooltipUI.SetText(header, description, position);
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
