using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DownloadPopupIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    
    private CanvasGroup _canvasGroup;

    private const float GRACE_TIME = 0.5f;
    private const float POPUP_TIME = 1f;

    private Coroutine showPopupCoroutine;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetText(string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;
    }

    public void ShowPopup()
    {
        if (showPopupCoroutine != null)
        {
            StopCoroutine(showPopupCoroutine);
        }
        
        showPopupCoroutine = StartCoroutine(ShowPopupCoroutine());
    }
    

    private IEnumerator ShowPopupCoroutine()
    {
        _canvasGroup.alpha = 1f;

        var timeLeft = GRACE_TIME;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timeLeft = POPUP_TIME;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            _canvasGroup.alpha = (timeLeft / POPUP_TIME);
            yield return new WaitForEndOfFrame();
        }
        
        _canvasGroup.alpha = 0f;

        showPopupCoroutine = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _canvasGroup.alpha = 0f;
    }
}
