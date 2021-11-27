using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsableWindow : MonoBehaviour
{

    private bool _isCollapsed = false;
    
    public float ExpandedHeight;
    public float CollapsedHeight;

    private RectTransform _rectTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

    }

    public void Toggle()
    {
        if (_isCollapsed)
        {
            Expand();
        }
        else
        {
            Collapse();
        }

        _isCollapsed = !_isCollapsed;
    }

    public void Collapse()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, CollapsedHeight);
    }

    public void Expand()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, ExpandedHeight);
    }
    
}
