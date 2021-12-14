using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;

public class StereonetDashboard : DashboardUI, IDashboardColorSwatch
{
    public static StereonetDashboard instance;

    [SerializeField] private StereonetsController stereonetController;
    [SerializeField] private StereonetFullscreenManager fullscreenManager;
    [SerializeField] protected Animator colorSwatchAnimator;

    protected override void OnAwake()
    {
        instance = this;
    }

    protected override void OnStart()
    {
    }

    // Adds a new stereonet to the world and to the UI
    protected override void OnAddCard(Transform newCard)
    {
        stereonetController.CreateStereonet();
        StereonetCamera.instance.UpdateStereonetImmediate(); // Ensures that the card image will be a blank stereonet

        // Adding a fullscreen click card event
        EventTrigger fullscreenTapEventTrigger = newCard.GetComponent<StereonetCard>().fullscreenEventTrigger;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener((eventData) =>
        {
            FullscreenCard();
        });
        fullscreenTapEventTrigger.triggers.Add(entry);
    }

    public override void OnOpenDashboard()
    {
        
    }

    protected override void OnSelectCard(Transform card, int cardIndex)
    {
        stereonetController.SelectStereonet(cardIndex);
    }

    protected override void OnDeleteSelectedCard(int cardIndex)
    {
        StereonetsController.instance.Delete(cardIndex);
    }

    protected override void OnDeleteAll()
    {
        stereonetController.RemoveAll();
        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.DeleteAll();
        }
    }

    // Updates the card of the given index's poles and image
    public void UpdateCard(int index, Texture newStereonetImage)
    {
        StereonetCard card = cards[index].GetComponent<StereonetCard>();
        card.SetStereonetImage(newStereonetImage);
    }

    public void FullscreenCard()
    {
        CloseSwatch();
        fullscreenManager.gameObject.SetActive(true);

        // Assign the current card's information to the fullscreenmanager
        StereonetCard card = selectedCard.GetComponent<StereonetCard>();
        Stereonet stereonet = stereonetController.GetStereonet(cards.IndexOf(selectedCard));
        fullscreenManager.UpdateValues(card.GetImage(), card.GetTitle(), stereonet);

        // TODO: only for rendering image with 2D values and not a camera screenshot
        //fullscreenManager.SetStereonet(stereonet.GetTwoDInfo(), stereonet.GetTwoDLine(), stereonet.GetTwoDFinalPoint());
    }

    protected override void OnClosedDashboard()
    {
        CloseSwatch();
        StereonetCamera.instance.UpdateStereonet();
    }

    public void ChangeSelectedCardColor(ColorSwatchButton colorButton)
    {
        selectedCard.GetComponent<StereonetCard>().SetColor(colorButton.color);
        StereonetsController.instance.currStereonet.ChangeFlagsMaterial(colorButton.color);
        CloseSwatch();
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
