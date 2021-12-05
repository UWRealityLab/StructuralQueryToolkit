using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerOverCursorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D MouseOverCursor;
    public Vector2 Offset;

    private bool isPointerInside = false;
    
    private void OnDestroy()
    {
        if (isPointerInside)
        {
            // Edge case where object is destroyed (by undo or some other action) while the pointer is still inside of it
            Cursor.SetCursor(ToolManager.instance.activeCursor, ToolManager.instance.activeCursorOffset, CursorMode.Auto);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(MouseOverCursor, Offset, CursorMode.Auto);
        isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(ToolManager.instance.activeCursor, ToolManager.instance.activeCursorOffset, CursorMode.Auto);
        isPointerInside = false;
    }
}
