using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopoProfileDashboard : DashboardUI
{
    public static TopoProfileDashboard instance;

    [SerializeField] private TopoProfileFullscreenManager fullScreenManager;

    private int _profilesCount = 0;
    
    protected override void OnAwake()
    {
        instance = this;
    }

    protected override void OnStart()
    {
    }

    protected override void OnAddCard(Transform newCard)
    {
        TopographicProfileTool.Instance.CreateNewProfile();
        _profilesCount++;
        var topoProfileCard = newCard.GetComponent<TopoProfileCard>();
        topoProfileCard.SetName($"Profile {_profilesCount}");
    }

    protected override void OnSelectCard(Transform card, int cardIndex)
    {
        
    }

    protected override void OnDeleteSelectedCard(int cardIndex)
    {
        TopographicProfileTool.instance.DeleteProfile(cardIndex);
    }

    protected override void OnDeleteAll()
    {
        TopographicProfileTool.instance.DeleteAll();
        _profilesCount = 0;
    }
    
    public override void OnOpenDashboard()
    {
        var topoProfileCard = selectedCard.GetComponent<TopoProfileCard>();
        topoProfileCard.SetGraph(TopographicProfileTool.instance.GetActiveGraphPoints(topoProfileCard.GetGraphDimensions()));
    }

    protected override void OnClosedDashboard()
    {
        TopographicProfileTool.instance.SelectProfile(selectedCardIndex);
    }

    public void OpenFullscreen(Transform card)
    {
        var topoProfileCard = card.GetComponent<TopoProfileCard>();
        var cardIndex = cards.IndexOf(card);
        fullScreenManager.Setup(topoProfileCard.GetTitle(), TopographicProfileTool.instance.GetProfile(cardIndex));
    }
}
