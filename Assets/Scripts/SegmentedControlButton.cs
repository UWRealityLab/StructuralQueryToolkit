using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SegmentedControlButton : MonoBehaviour
{
    public float transitionTime = 0.25f;

    public Button sliderButton;
    public RectTransform sliderRect;

    private bool isLeftSelected = true;

    [Header("Left Button")]
    public Image leftIcon;
    public TextMeshProUGUI leftText;
    public UnityEvent leftEvent;

    [Header("Right Button")]
    public Image rightIcon;
    public TextMeshProUGUI rightText;
    public UnityEvent rightEvent;

    [Header("Colors")]
    public Color selectedColor;
    public Color deselectedColor;

    private void Start()
    {
        sliderButton.onClick.AddListener(PressButton);
    }

    public void PressButton() => StartCoroutine(PressButtonCoroutine());

    private bool isTransitioning = false;
    private IEnumerator PressButtonCoroutine()
    {
        if (isTransitioning)
        {
            yield break;
        }
        isTransitioning = true;

        float duration = 0f;

        isLeftSelected = !isLeftSelected;

        if (isLeftSelected)
        {
            leftEvent.Invoke();
            Vector2 buttonDesiredPos = new Vector2(sliderRect.anchoredPosition.x - sliderRect.sizeDelta.x, sliderRect.anchoredPosition.y);
            while (duration < transitionTime)
            {
                duration += Time.deltaTime;
                var t = duration / transitionTime;
                var lerpPct = t * t * t * (t * (6f * t - 15f) + 10f);
                sliderRect.anchoredPosition = Vector2.Lerp(sliderRect.anchoredPosition, buttonDesiredPos, lerpPct);
                leftIcon.color = Color.Lerp(leftIcon.color, selectedColor, lerpPct);
                rightIcon.color = Color.Lerp(rightIcon.color, deselectedColor, lerpPct);
                leftText.color = Color.Lerp(leftText.color, selectedColor, lerpPct);
                rightText.color = Color.Lerp(rightText.color, deselectedColor, lerpPct);
                yield return new WaitForEndOfFrame();
            }
            sliderRect.anchoredPosition = buttonDesiredPos;
            leftIcon.color = selectedColor;
            rightIcon.color = deselectedColor;
            leftText.color = selectedColor;
            rightText.color = deselectedColor;
        }
        else
        {
            // Moving right
            rightEvent.Invoke();
            Vector2 buttonDesiredPos = new Vector2(sliderRect.anchoredPosition.x + sliderRect.sizeDelta.x, sliderRect.anchoredPosition.y);
            while (duration < transitionTime)
            {
                duration += Time.deltaTime;
                var t = duration / transitionTime;
                var lerpPct = t * t * t * (t * (6f * t - 15f) + 10f);

                sliderRect.anchoredPosition = Vector2.Lerp(sliderRect.anchoredPosition, buttonDesiredPos, lerpPct);
                leftIcon.color = Color.Lerp(leftIcon.color, deselectedColor, lerpPct);
                rightIcon.color = Color.Lerp(rightIcon.color, selectedColor, lerpPct);
                leftText.color = Color.Lerp(leftText.color, deselectedColor, lerpPct);
                rightText.color = Color.Lerp(rightText.color, selectedColor, lerpPct);
                yield return new WaitForEndOfFrame();
            }
            sliderRect.anchoredPosition = buttonDesiredPos;
            leftIcon.color = deselectedColor;
            rightIcon.color = selectedColor;
            leftText.color = deselectedColor;
            rightText.color = selectedColor;
        }

        isTransitioning = false;
    }
}
