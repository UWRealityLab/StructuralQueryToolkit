using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitTrigger : MonoBehaviour
{
    float fadeTime = 1f;
    [SerializeField] Image bgImage;
    [SerializeField] FirstPersonController player;

    private void OnTriggerEnter(Collider other)
    {
        player.enabled = false;

        var loadOperation = SceneManager.LoadSceneAsync(0);
        loadOperation.allowSceneActivation = false;

        StartCoroutine(FadeToWhite(loadOperation));
    }


    IEnumerator FadeToWhite(AsyncOperation op)
    {
        float timeLeft = fadeTime;
        var lerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / timeLeft) * Time.deltaTime);

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            bgImage.color = Color.Lerp(bgImage.color, Color.white, lerpPct);
            yield return new WaitForEndOfFrame();
        }
        bgImage.color = Color.white;
        //print("Fade finished");

        op.allowSceneActivation = true;
    }
}