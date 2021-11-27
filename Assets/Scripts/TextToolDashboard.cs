using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToolDashboard : DashboardUI, IDashboardColorSwatch
{
    public static TextToolDashboard instance;

    [SerializeField] protected Animator colorSwatchAnimator;

    protected override void OnAwake()
    {
        instance = this;
    }

    protected override void OnStart()
    {
    }

    protected override void OnAddCard(Transform newCard)
    {
        TextTool.Instance.CreateNewTextGroup();
    }

    protected override void OnSelectCard(Transform card, int cardIndex)
    {
        TextTool.instance.SelectTextGroup(cardIndex);
    }

    protected override void OnDeleteSelectedCard(int cardIndex)
    {
        TextTool.instance.DeleteActiveTextGroup();
    }

    protected override void OnDeleteAll()
    {
        TextTool.instance.DeleteAll();
    }
    
    public override void OnOpenDashboard()
    {
        
    }

    protected override void OnClosedDashboard()
    {
        
    }

    public void ChangeSelectedCardColor(ColorSwatchButton colorButton)
    {
        // No color change functionality for text groups atm
    }

    public void OpenSwatch()
    {
        colorSwatchAnimator.SetBool("isToggled", true);
    }

    public void CloseSwatch()
    {
        colorSwatchAnimator.SetBool("isToggled", false);
    }
}
