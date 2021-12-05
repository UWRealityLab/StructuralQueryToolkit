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

    private Vector2 _leftPos;
    private Vector2 _rightPos;

    private void Start()
    {
        sliderButton.onClick.AddListener(PressButton);
        _leftPos = new Vector2(sliderRect.anchoredPosition.x, sliderRect.anchoredPosition.y);
        _rightPos = new Vector2(sliderRect.anchoredPosition.x + sliderRect.sizeDelta.x, sliderRect.anchoredPosition.y);
    }

    public void PressButton()
    {
        if (isTransitioning)
        {
            return;
        }
        isLeftSelected = !isLeftSelected;

        StartCoroutine(PressButtonCoroutine());
    }

    private bool isTransitioning = false;
    private IEnumerator PressButtonCoroutine()
    {
        isTransitioning = true;
        float duration = 0f;

        if (isLeftSelected)
        {
            leftEvent.Invoke();
            while (duration < transitionTime)
            {
                duration += Time.deltaTime;
                var t = duration / transitionTime;
                var lerpPct = t * t * t * (t * (6f * t - 15f) + 10f);
                sliderRect.anchoredPosition = Vector2.Lerp(sliderRect.anchoredPosition, _leftPos, lerpPct);
                leftIcon.color = Color.Lerp(leftIcon.color, selectedColor, lerpPct);
                rightIcon.color = Color.Lerp(rightIcon.color, deselectedColor, lerpPct);
                leftText.color = Color.Lerp(leftText.color, selectedColor, lerpPct);
                rightText.color = Color.Lerp(rightText.color, deselectedColor, lerpPct);
                yield return new WaitForEndOfFrame();
            }
            sliderRect.anchoredPosition = _leftPos;
            leftIcon.color = selectedColor;
            rightIcon.color = deselectedColor;
            leftText.color = selectedColor;
            rightText.color = deselectedColor;
        }
        else
        {
            // Moving right
            rightEvent.Invoke();
            while (duration < transitionTime)
            {
                duration += Time.deltaTime;
                var t = duration / transitionTime;
                var lerpPct = t * t * t * (t * (6f * t - 15f) + 10f);

                sliderRect.anchoredPosition = Vector2.Lerp(sliderRect.anchoredPosition, _rightPos, lerpPct);
                leftIcon.color = Color.Lerp(leftIcon.color, deselectedColor, lerpPct);
                rightIcon.color = Color.Lerp(rightIcon.color, selectedColor, lerpPct);
                leftText.color = Color.Lerp(leftText.color, deselectedColor, lerpPct);
                rightText.color = Color.Lerp(rightText.color, selectedColor, lerpPct);
                yield return new WaitForEndOfFrame();
            }
            sliderRect.anchoredPosition = _rightPos;
            leftIcon.color = deselectedColor;
            rightIcon.color = selectedColor;
            leftText.color = deselectedColor;
            rightText.color = selectedColor;
        }

        isTransitioning = false;
    }

    public void SetState(bool isLeft)
    {
        StopAllCoroutines();
        isLeftSelected = isLeft;
        StartCoroutine(PressButtonCoroutine());
    }
}
