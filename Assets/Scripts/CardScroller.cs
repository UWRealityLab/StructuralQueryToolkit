using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CardScroller : MonoBehaviour {

    public List<RectTransform> cards;
    RectTransform currCard;
    int currCardIndex;

    void Awake() {
        cards = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++) {
            RectTransform tempCard = transform.GetChild(i).GetComponent<RectTransform>();
            cards.Add(tempCard);
        }
        currCard = cards[0];

    }

    // Start is called before the first frame update
    void Start() {
        currCardIndex = 0;
    }

    public void ShowUI() {
        currCard.gameObject.SetActive(true);  // Enables the first image
    }

    public void NextCard() {
        if (currCardIndex + 1 >= cards.Count) {
            return;
        }
        currCardIndex++;

        currCard.GetComponent<Animator>().SetTrigger("switchLeftTrigger");
        StartCoroutine(RemoveCardTransition(currCard.gameObject));
        currCard = cards[currCardIndex];
        currCard.gameObject.SetActive(true);
        currCard.GetComponent<Animator>().SetTrigger("insertFromRightTrigger");

    }

    public void PrevCard() {
        if (currCardIndex - 1 < 0) {
            return;
        }
        currCardIndex--;

        currCard.GetComponent<Animator>().SetTrigger("switchRightTrigger");
        StartCoroutine(RemoveCardTransition(currCard.gameObject));
        currCard = cards[currCardIndex];
        currCard.gameObject.SetActive(true);
        currCard.GetComponent<Animator>().SetTrigger("insertFromLeftTrigger");

    }

    IEnumerator RemoveCardTransition(GameObject cardGameObject) {
        
        yield return new WaitForSeconds(0.5f);
        cardGameObject.SetActive(false);
    }
}
