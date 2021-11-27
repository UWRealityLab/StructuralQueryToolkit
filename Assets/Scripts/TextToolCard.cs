using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class TextToolCard : MonoBehaviour, IDashboardCard
{
    [SerializeField] private ToggleButton SelectedIndicatorIcon;
    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private Image colorIcon;
    [SerializeField] private RawImage deleteIcon;
    [SerializeField] private ToggleButton hideToggleButton;

    private void OnEnable()
    {
        // Since the player can make the text world visible again by placing a new text when the group was invisible, 
        // we'll need to update the state of the visibility button
        var index = TextToolDashboard.instance.GetCardIndex(transform);
        if (index < 0)
        {
            return;
        }
        var state = TextTool.instance.GetVisibilityState(index);
        hideToggleButton.SetState(state);
    }

    public string GetTitle()
    {
        return titleInputField.text;
    }
    
    public void Rename()
    {
        titleInputField.ActivateInputField();
    }

    public void SetColor(Color color)
    {
        colorIcon.color = color;
    }
    
    public void Select()
    {
        deleteIcon.enabled = true;
        SelectedIndicatorIcon.SetState(true);
        
        titleInputField.ActivateInputField();
    }
    
    public void Deselect()
    {
        deleteIcon.enabled = false;
        SelectedIndicatorIcon.SetState(false);
    }

    public void Delete()
    {
        TextToolDashboard.instance.DeleteSelectedCard();
    }
    
    public void ToggleVisibility()
    {
        hideToggleButton.Toggle();
        var index = TextToolDashboard.instance.GetCardIndex(transform);
        TextTool.instance.SetTextGroupVisibility(index, hideToggleButton.GetState());
    }

    public void OpenColorSwatch()
    {
        TextToolDashboard.instance.OpenSwatch();
    }
}
