using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[ExecuteAlways]
[RequireComponent(typeof(HorizontalLayoutGroup))]
public class CustomToolsDrawer : MonoBehaviour
{
    private static CustomToolsDrawer _instance;
    public static CustomToolsDrawer Instance
    {
        set
        {
            _instance = value;
        }
        get
        {
            if (_instance)
            {
                return _instance;
            }
            return FindObjectOfType<CustomToolsDrawer>().GetComponent<CustomToolsDrawer>();
        }
    }

    private HorizontalLayoutGroup _LayoutGroup;
    public HorizontalLayoutGroup LayoutGroup
    {
        set
        {
            _LayoutGroup = value;
        }
        get
        {
            if (_LayoutGroup == null)
            {
                return GetComponent<HorizontalLayoutGroup>();
            }

            return _LayoutGroup;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    
    public void AddObjectToDrawer(Transform obj)
    {
        obj.SetParent(transform);
    }

    private void OnTransformChildrenChanged()
    {
        LayoutGroup.enabled = transform.childCount != 0;
    }
}
