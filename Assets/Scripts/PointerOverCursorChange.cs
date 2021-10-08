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

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(MouseOverCursor, Offset, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(ToolManager.instance.activeCursor, ToolManager.instance.activeCursorOffset, CursorMode.Auto);
    }
}
