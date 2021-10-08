using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;

public class StereonetDashboard : MonoBehaviour
{
    public static StereonetDashboard singleton;

    Animator animator;
    [SerializeField] StereonetsController stereonetController;
    [SerializeField] Transform stereonetCardsList;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] RectTransform addCard;
    [SerializeField] StereonetFullscreenManager fullscreenManager;
    [SerializeField] Animator colorSwatchAnimator;

    public List<Transform> cards;
    const int MAX_CARDS = 6;

    // Hardcoded card positions
    Vector2[] cardPositions = {
        new Vector2(-630.5f, 150f), new Vector2(0f, 150f), new Vector2(630.5f, 150f),
        new Vector2(-630.5f, -300f), new Vector2(0f, -300f), new Vector2(630.5f, -300f)
    };

    public Transform selectedCard;

    private void Awake()
    {
        singleton = this;
        animator = GetComponent<Animator>();
        cards = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AddStereonet();
        gameObject.SetActive(false);
    }

    // Adds a new stereonet to the world and to the UI
    public void AddStereonet()
    {
        if (LatestMeasurementUI.instance != null)
        {
            LatestMeasurementUI.instance.Clear();
        }

        if (cards.Count >= MAX_CARDS)
        {
            return;
        }
        // Create a new card in the canvas
        Transform newCard = Instantiate(cardPrefab, stereonetCardsList).transform;
        cards.Add(newCard);

        newCard.GetComponent<RectTransform>().anchoredPosition = cardPositions[cards.Count - 1];

        // Adding click on a card event
        EventTrigger cardEventTrigger = newCard.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener((eventData) =>
        {
            SelectCard(newCard);
        });
        cardEventTrigger.triggers.Add(entry);

        // Adding a fullscreen click card event
        EventTrigger fullscreenTapEventTrigger = newCard.GetComponent<StereonetCard>().fullscreenEventTrigger;
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener((eventData) =>
        {
            FullscreenCard();
        });
        fullscreenTapEventTrigger.triggers.Add(entry);


        if (cards.Count != MAX_CARDS)
        {
            addCard.anchoredPosition = cardPositions[cards.Count];
        }
        else
        {
            addCard.anchoredPosition = Vector2.positiveInfinity;
        }
        stereonetController.CreateStereonet();
        SelectCard(newCard);
        StereonetCamera.instance.cam.Render(); // Ensures that the card image will be a blank stereonet
    }


    public void SelectCard(Transform card)
    {
        if (LatestMeasurementUI.instance != null)
        {
            LatestMeasurementUI.instance.Clear();
        }

        int index = cards.IndexOf(card);
        if (selectedCard != null)
        {
            StereonetCard deselectedCard = selectedCard.GetComponent<StereonetCard>();
            deselectedCard.Deselect();
        }
        selectedCard = cards[index];
        StereonetCard tempCard = selectedCard.GetComponent<StereonetCard>();
        tempCard.Select();

        stereonetController.SelectStereonet(index);
    }

    // Deletes the selected card and assigns a new card (adjacent card)
    // Assumes there is at least 1 card
    public void DeleteSelectedCard()
    {
        LatestMeasurementUI.instance.Clear();


        int cardIndex = cards.IndexOf(selectedCard);
        StereonetsController.singleton.Delete(cardIndex);
        cards.Remove(selectedCard);
        Destroy(selectedCard.gameObject);

        // Selects the adjacent card (if applicable)
        if (cards.Count > 0)
        {
            if (cards.Count == cardIndex)
            {
                SelectCard(cards[cardIndex - 1]); // Move one space back if the deleted card was last
            }
            else
            {
                SelectCard(cards[cardIndex]);
            }
        }

        UpdateCardLocations();
        addCard.anchoredPosition = cardPositions[cards.Count];

        if (cards.Count == 0) {
            addCard.anchoredPosition = cardPositions[0];
            AddStereonet();
        }
    }

    public void RemoveAll()
    {
        LatestMeasurementUI.instance.Clear();

        stereonetController.RemoveAll();
        foreach (Transform card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();

        addCard.anchoredPosition = cardPositions[0];
        AddStereonet();

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.DeleteAll();
        }
    }
    
    // Updates the card of the given index's poles and image
    public void UpdateCard(int index, int numPoles, Texture newStereonetImage)
    {
        StereonetCard card = cards[index].GetComponent<StereonetCard>();
        card.SetStereonetImage(newStereonetImage);
    }

    // When a card is deleted, reformat the cards in the scene 
    void UpdateCardLocations()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<RectTransform>().anchoredPosition = cardPositions[i];
        }
    }

    public void FullscreenCard()
    {
        CloseSwatch();
        fullscreenManager.gameObject.SetActive(true);

        // Assign the current card's information to the fullscreenmanager
        StereonetCard card = selectedCard.GetComponent<StereonetCard>();
        Stereonet stereonet = stereonetController.GetStereonet(cards.IndexOf(selectedCard));
        fullscreenManager.UpdateValues(card.GetImage(), card.GetTitle(), stereonet);

        // TODO 

        // TODO: only for rendering image with 2D values and not a camera screenshot
        //fullscreenManager.SetStereonet(stereonet.GetTwoDInfo(), stereonet.GetTwoDLine(), stereonet.GetTwoDFinalPoint());
    }

    public void Exit()
    {
        CloseSwatch();
        StartCoroutine(ExitCoroutine());
    }

    IEnumerator ExitCoroutine()
    {
        animator.SetTrigger("exitTrigger");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
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
