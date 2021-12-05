using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RulerToolCard : MonoBehaviour, IDashboardCard
{
    [SerializeField] private ToggleButton SelectedIndicatorIcon;
    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private RawImage deleteIcon;
    [SerializeField] private ToggleButton hideToggleButton;

    public string GetTitle()
    {
        return titleInputField.text;
    }
    
    public void Rename()
    {
        titleInputField.ActivateInputField();
    }

    public void Select()
    {
        deleteIcon.enabled = true;
        SelectedIndicatorIcon.SetState(true);
    }
    
    public void Deselect()
    {
        deleteIcon.enabled = false;
        SelectedIndicatorIcon.SetState(false);
    }

    public void Delete()
    {
        RulerDashboard.instance.DeleteSelectedCard();
    }
    
    public void ToggleVisibility()
    {
        hideToggleButton.Toggle();
        var index = RulerDashboard.instance.GetCardIndex(transform);
        RulerPlotting.instance.SetRulerVisibility(index, hideToggleButton.GetState());
    }
}
