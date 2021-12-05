using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class DownloadPopupIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    
    private CanvasGroup canvasRenderer;

    private const float GRACE_TIME = 0.5f;
    private const float POPUP_TIME = 1f;

    private Coroutine showPopupCoroutine;

    private void Awake()
    {
        canvasRenderer = GetComponent<CanvasGroup>();
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
        canvasRenderer.alpha = 1f;

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
            canvasRenderer.alpha = (timeLeft / POPUP_TIME);
            yield return new WaitForEndOfFrame();
        }
        
        canvasRenderer.alpha = 0f;

        showPopupCoroutine = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        canvasRenderer.alpha = 0f;
    }
}
