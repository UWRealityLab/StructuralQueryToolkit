using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CardUI : MonoBehaviour
{
    public float imageWidthDecrease = 100f;
    public float headerPadding = 60f;
    public float bodyPadding = 20f;
    [Range(0f, 1f)] public float alpha = 0.5f; // Sets the alpha of every element inside this card (for convienence when animating transitions)
    public bool isAnimating = false; // On runs certain appearance changes when in animations

    public RectTransform canvasRect;
    public RectTransform transformRect;
    public RectTransform imageRect;
    public TextMeshProUGUI headerRect;
    public TextMeshProUGUI bodyRect;

    public Image bgImage;
    public Image mainImage;

    private Animator animator;

    void Awake() {
        //canvasRect = transform.parent.parent.GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
        transformRect = GetComponent<RectTransform>();
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        headerRect = texts[0];
        bodyRect = texts[1];
        bgImage = GetComponent<Image>();
        mainImage = imageRect.transform.GetComponent<Image>();
    }

    private void Start() {
        UpdateCardHeight();
    }

    private void Update() {
        animator.enabled = isAnimating;
        UpdateCardAlpha();
    }

    private void UpdateCardAlpha() {
        bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, alpha);
        mainImage.color = new Color(mainImage.color.r, mainImage.color.g, mainImage.color.b, alpha);
        headerRect.color = new Color(headerRect.color.r, headerRect.color.g, headerRect.color.b, alpha);
        bodyRect.color = new Color(bodyRect.color.r, bodyRect.color.g, bodyRect.color.b, alpha);

    }


    public void UpdateCardHeight() {
        imageRect.sizeDelta = new Vector2(transformRect.rect.width - imageWidthDecrease, imageRect.rect.height);
        headerRect.rectTransform.anchoredPosition = new Vector2(headerRect.rectTransform.anchoredPosition.x, -imageRect.rect.height - headerPadding);
        bodyRect.rectTransform.anchoredPosition = new Vector2(bodyRect.rectTransform.anchoredPosition.x, headerRect.rectTransform.anchoredPosition.y - headerRect.rectTransform.rect.height - bodyPadding);

        // The card's height will be at minimum the fit the entire camera's height
        //float cardHeight = Mathf.Max(canvasRect.rect.height, imageRect.rect.height + headerRect.renderedHeight + bodyRect.renderedHeight + headerPadding + bodyPadding + 100f);
        //transformRect.sizeDelta = new Vector3(transformRect.sizeDelta.x, cardHeight);
    }

    private void OnValidate() {
        UpdateCardHeight();
    }
}
