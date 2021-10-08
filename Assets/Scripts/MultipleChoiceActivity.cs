using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MultipleChoiceActivity : MonoBehaviour {

    [SerializeField] Image overlay;
    [SerializeField] Transform answers;
    [SerializeField] GameObject introPanel;
    [SerializeField] GameObject questionPanel;
    [SerializeField] GameObject informationPanel;
    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject downArrow;

    [Range(0.1f, 2f)] public float colorRevealTime = 1f;

    List<Transform> incorrectCards;

    Color CORRECT_COLOR = new Color32(69, 156, 78, 255);
    Color WRONG_COLOR = new Color32(156, 72, 69, 255);
    Color INVISIBLE_COLOR = new Color32(0, 0, 0, 0);

    private Animator animator;


    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        questionPanel.SetActive(false);
        informationPanel.SetActive(false);
        incorrectCards = new List<Transform>();
        for (int i = 0; i < answers.childCount; i++) {
            Transform tempChild = answers.GetChild(i);
            if (answers.GetChild(i).tag.Equals("Incorrect Answer")) {
                incorrectCards.Add(tempChild);
            }
        }
    }

    private void Start() {
        animator.SetTrigger("startActivityTrigger");
    }
    
    public void MoveQuestionDown() {
        animator.SetTrigger("moveQuestionDownTrigger");
    }

    public void MoveQuestionUp() {
        animator.SetTrigger("moveQuestionUpTrigger");
    }

    public void ShowQuestion() {
        StartCoroutine(ShowQuestionCoroutine());
    }

    IEnumerator ShowQuestionCoroutine() {
        animator.SetTrigger("showQuestionsTrigger");

        yield return new WaitForSeconds(0.5f);
        introPanel.SetActive(false);
    }

    public void WinActivity(Image card) {
        StartCoroutine(ShowCorrectAnswer(card));
    }

    public void WrongAnswer(Image card) {
        StartCoroutine(HighlightCard(card, WRONG_COLOR));
    }

    IEnumerator HighlightCard(Image card, Color color) {
        float timeLeft = colorRevealTime;
        var lerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / timeLeft) * Time.deltaTime);

        TextMeshProUGUI cardText = card.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        while (timeLeft > 0f) {
            timeLeft -= Time.deltaTime;
            card.color = Color32.Lerp(card.color, color, lerpPct);
            //cardText.color = Color.Lerp(cardText.color, Color.white, lerpPct);

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ShowCorrectAnswer(Image correctCard) {
        float timeLeft = colorRevealTime;
        var lerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / timeLeft) * Time.deltaTime);

        List<TextMeshProUGUI> incorrectTexts = new List<TextMeshProUGUI>();
        List<Image> incorrectCardsImages = new List<Image>();
        foreach (Transform incorrectCardTransform in incorrectCards) {
            incorrectTexts.Add(incorrectCardTransform.GetComponentInChildren<TextMeshProUGUI>());
            incorrectCardsImages.Add(incorrectCardTransform.GetComponent<Image>());
            incorrectCardTransform.GetComponent<Button>().enabled = false;
        }

        // Highlights the correct answer and makes every other card invisible
        //TextMeshProUGUI correctCardText = correctCard.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        while (timeLeft > 0f) {
            timeLeft -= Time.deltaTime;
            correctCard.color = Color32.Lerp(correctCard.color, CORRECT_COLOR, lerpPct);
            //correctCardText.color = Color.Lerp(correctCardText.color, Color.white, lerpPct);
            
            for (int i = 0; i < incorrectCards.Count; i++) {
                incorrectTexts[i].color = Color32.Lerp(incorrectTexts[i].color, INVISIBLE_COLOR, lerpPct);
                incorrectCardsImages[i].color = Color32.Lerp(incorrectCardsImages[i].color, INVISIBLE_COLOR, lerpPct);
            }
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(ShowInfoCard());

    }

    IEnumerator ShowInfoCard() {
        animator.SetTrigger("showInfoTrigger");
        informationPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        questionPanel.SetActive(false);
    }

    private void OnDisable() {
        Reset();
    }

    public void Reset() {
        overlay.color = new Color32(255, 255, 255, 100);
        questionPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        downArrow.SetActive(true);
        upArrow.SetActive(false);
    }

}
