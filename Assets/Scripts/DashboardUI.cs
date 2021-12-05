using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DashboardUI : MonoBehaviour
{
    protected Animator animator;
    [SerializeField] protected Transform cardsContainer;
    [SerializeField] protected GameObject cardPrefab;
    [SerializeField] protected RectTransform addCard;

    protected List<Transform> cards;
    protected Transform selectedCard;
    protected int selectedCardIndex;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cards = new List<Transform>();
        OnAwake();
    }
    
    protected abstract void OnAwake();

    // Start is called before the first frame update
    void Start()
    {
        AddCard();
        gameObject.SetActive(false);
        OnStart();
    }

    protected abstract void OnStart();

    public void OpenDashboard()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("showTrigger");
        GameController.instance.DisablePlayer();
        ToolManager.instance.DisableActiveTool();
        OnOpenDashboard();
    }

    public abstract void OnOpenDashboard();

    public void CloseDashboard()
    {
        GameController.instance.EnablePlayer();
        ToolManager.instance.EnableActiveTool();
        OnClosedDashboard();
        StartCoroutine(ExitCoroutine());
    }

    // Adds a new card and its respective world representation to the world
    public void AddCard()
    {
        if (LatestMeasurementUI.instance != null)
        {
            LatestMeasurementUI.instance.Clear();
        }

        // Create a new card in the canvas
        Transform newCard = Instantiate(cardPrefab, cardsContainer).transform;
        addCard.SetAsLastSibling();
        cards.Add(newCard);

        // Adding click on a card event
        var cardButtonComponent = newCard.GetComponent<Button>();
        cardButtonComponent.onClick.AddListener(() =>
        {
            SelectCard(newCard);
        });
        
        OnAddCard(newCard);
        SelectCard(newCard);
    }
    
    protected abstract void OnAddCard(Transform newCard);

    public void SelectCard(Transform card)
    {
        if (LatestMeasurementUI.instance != null)
        {
            LatestMeasurementUI.instance.Clear();
        }

        int index = cards.IndexOf(card);

        if (selectedCard != null)
        {
            IDashboardCard deselectedCard = selectedCard.GetComponent<IDashboardCard>();
            deselectedCard.Deselect();
        }
        selectedCard = cards[index];
        selectedCardIndex = index;
        IDashboardCard tempCard = selectedCard.GetComponent<IDashboardCard>();
        tempCard.Select();

        OnSelectCard(card, index);
    }
    
    protected abstract void OnSelectCard(Transform card, int cardIndex);

    // Deletes the selected card and assigns a new card (adjacent card)
    // Assumes there is at least 1 card
    public void DeleteSelectedCard()
    {
        if (LatestMeasurementUI.instance != null)
        {
            LatestMeasurementUI.instance.Clear();
        }

        int cardIndex = cards.IndexOf(selectedCard);
        OnDeleteSelectedCard(cardIndex); 
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

        if (cards.Count == 0) {
            AddCard();
        }
    }

    protected abstract void OnDeleteSelectedCard(int cardIndex);


    public int GetCardIndex(Transform card)
    {
        return cards.IndexOf(card);
    }
    
    public void RemoveAll()
    {
        LatestMeasurementUI.instance.Clear();

        foreach (Transform card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();

        OnDeleteAll();
        AddCard();
    }
    
    protected abstract void OnDeleteAll();
    
    protected abstract void OnClosedDashboard();

    IEnumerator ExitCoroutine()
    {
        animator.SetTrigger("exitTrigger");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}

public interface IDashboardColorSwatch
{

    public void ChangeSelectedCardColor(ColorSwatchButton colorButton);
    
    public void OpenSwatch();

    public void CloseSwatch();
}
