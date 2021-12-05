using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[Serializable]
public class TopoProfileCard : MonoBehaviour, IDashboardCard
{
    [SerializeField] private UILineRenderer graphLineRenderer;
    [SerializeField] private ToggleButton selectedIndicatorIcon;
    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private RawImage deleteIcon;
    [SerializeField] private ToggleButton hideToggleButton;

    private void OnEnable()
    {
        // Since the player can make the text world visible again by placing a new text when the group was invisible, 
        // we'll need to update the state of the visibility button
        var index = TopoProfileDashboard.instance.GetCardIndex(transform);
        if (index < 0)
        {
            return;
        }
        var state = TopographicProfileTool.instance.GetProfileVisibility(index);
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

    public void SetName(string name)
    {
        titleInputField.text = name;
    }

    public void Select()
    {
        deleteIcon.enabled = true;
        selectedIndicatorIcon.SetState(true);
    }
    
    public void Deselect()
    {
        deleteIcon.enabled = false;
        selectedIndicatorIcon.SetState(false);
    }

    public void Delete()
    {
        TopoProfileDashboard.instance.DeleteSelectedCard();
    }
    
    public void ToggleVisibility()
    {
        hideToggleButton.Toggle();
        var index = TopoProfileDashboard.instance.GetCardIndex(transform);
        TopographicProfileTool.instance.SetProfileVisibility(index, hideToggleButton.GetState());
    }

    public void SetGraph(Vector2[] graphPoints)
    {
        if (graphPoints.Length > 0)
        {
            // To stop a weird artifact occuring from the graph seen in the dashboard, give the graph one point
            graphLineRenderer.Points = graphPoints;
        }
    }

    public Vector2 GetGraphDimensions()
    {
        return graphLineRenderer.rectTransform.rect.size;
    }

    public void FullscreenCard()
    {
        TopoProfileDashboard.instance.OpenFullscreen(transform);
    }
}
