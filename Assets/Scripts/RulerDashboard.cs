using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerDashboard : DashboardUI
{
    public static RulerDashboard instance;

    protected override void OnAwake()
    {
        instance = this;
    }

    protected override void OnStart()
    {
    }

    protected override void OnAddCard(Transform newCardTrans)
    {
        RulerPlotting.Instance.NewRuler();
    }

    protected override void OnSelectCard(Transform card, int cardIndex)
    {
        RulerPlotting.instance.SelectRuler(cardIndex);
    }

    protected override void OnDeleteSelectedCard(int cardIndex)
    {
        RulerPlotting.instance.DeleteSelectedRuler();
    }

    protected override void OnDeleteAll()
    {
        RulerPlotting.instance.DeleteAll();
    }
    
    public override void OnOpenDashboard()
    {
        
    }

    protected override void OnClosedDashboard()
    {
        
    }
}