using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.Events;
using UnityEngine.UI;

public class StereonetDashboard : DashboardUI, IDashboardColorSwatch
{
    public static StereonetDashboard instance;

    public UnityEvent<StereonetCard> OnAddStereonetCard;

    [SerializeField] private StereonetsController stereonetController;
    [SerializeField] private StereonetFullscreenManager fullscreenManager;
    [SerializeField] protected Animator colorSwatchAnimator;
    
    private static readonly int IsToggledTrigger = Animator.StringToHash("isToggled");

    protected override void OnAwake()
    {
        instance = this;
    }

    protected override void OnStart()
    {
    }

    // Adds a new stereonet to the world and to the UI
    protected override void OnAddCard(Transform newCardTrans)
    {
        stereonetController.CreateStereonet();

        if (StereonetCamera.instance)
        {
            StereonetCamera.instance.UpdateStereonetImmediate(); // Ensures that the card image will be a blank stereonet
        }

        // Adding a fullscreen click card event
        var newCard = newCardTrans.GetComponent<StereonetCard>();
        EventTrigger fullscreenTapEventTrigger = newCard.fullscreenEventTrigger;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener((eventData) =>
        {
            FullscreenCard();
        });
        fullscreenTapEventTrigger.triggers.Add(entry);
        
        if (GameController.instance.IsVR)
        {
            newCard.SetStereonet2DImage(stereonetController.currStereonet as Stereonet2D);
        }
        OnAddStereonetCard.Invoke(newCard);
    }

    public override void OnOpenDashboard()
    {
        StereonetsController.instance.UpdateStereonetDashboard();

        if (GameController.instance.IsVR)
        {
            gameObject.SetActive(!fullscreenManager.gameObject.activeSelf);
        }
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
        gameObject.SetActive(false);

        // Assign the current card's information to the fullscreenmanager
        StereonetCard card = selectedCard.GetComponent<StereonetCard>();
        Stereonet stereonet = stereonetController.GetStereonet(cards.IndexOf(selectedCard));
        
        fullscreenManager.OnCloseEvent.AddListener(() =>
        {
            card.SetStereonet2DImage(stereonet as Stereonet2D);
            gameObject.SetActive(true);
            fullscreenManager.OnCloseEvent.RemoveAllListeners();
        });

        fullscreenManager.UpdateValues(card.GetTitle(), stereonet as Stereonet2D);
    }
    

    protected override void OnClosedDashboard()
    {
        CloseSwatch();
        
        var currStereonet = StereonetsController.instance.currStereonet as Stereonet2D;

        if (!GameController.instance.IsVR)
        {
            currStereonet.MoveStereonetUI(StereonetCanvas.Instance.Stereonet2DContainer.transform);
        }
    }

    public void ChangeSelectedCardColor(ColorSwatchButton colorButton)
    {
        selectedCard.GetComponent<StereonetCard>().SetColor(colorButton.color);
        StereonetsController.instance.currStereonet.ChangeFlagsMaterial(colorButton.color);
        CloseSwatch();
    }

    public void OpenSwatch()
    {
        colorSwatchAnimator.SetBool(IsToggledTrigger, true);
    }

    public void CloseSwatch()
    {
        colorSwatchAnimator.SetBool(IsToggledTrigger, false);
    }
}
