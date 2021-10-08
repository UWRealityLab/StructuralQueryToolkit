using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaleolatitudeActivity : MonoBehaviour {

    [SerializeField] GameObject introCard;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] RawImage bordersImage;
    [SerializeField] GameObject mapTexts;
    [SerializeField] RectTransform correctPoint;
    [SerializeField] GameObject map;
    [SerializeField] float distanceThreshold = 3f;
    [SerializeField] RectTransform marker;

    [SerializeField] GameObject informationPanel1;
    [SerializeField] GameObject informationPanel2;

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip correct;
    [SerializeField] AudioClip incorrect;

    Vector2 correctPos;
    bool isToggled = false; // If the activity is turned on
    private Animator animator;
    private Animator markerAnimator;
    private RawImage markerImage;

    // Start is called before the first frame update
    void Start() {
        markerImage = marker.GetComponent<RawImage>();
        markerAnimator = marker.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        correctPos = correctPoint.anchoredPosition;
        animator.SetTrigger("startActivityTrigger");
    }

    // Update is called once per frame
    void Update() {
        if (isToggled) {
            FindPointActivity();
        }
    }

    void FindPointActivity() {
        if (Input.GetMouseButtonUp(0)) {
            correctPos = correctPoint.anchoredPosition;

            Vector2 touchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out touchPos);
            float dist = Vector2.Distance(touchPos, correctPos);
            Debug.Log(dist);
            marker.anchoredPosition = touchPos;
            markerAnimator.SetTrigger("transitionInTrigger");

            if (dist <= distanceThreshold) {
                //WinActivity();
            } else {
                // Play incorrect sound
                //source.PlayOneShot(incorrect);

                // Increasingly show the yellow lines in the map as a hint
                StartCoroutine(IncreaseBorderAlpha());
            }
        }
    }

    bool isIncreasingBorderAlpha = false;
    IEnumerator IncreaseBorderAlpha()
    {
        if (isIncreasingBorderAlpha)
        {
            yield break;
        }
        isIncreasingBorderAlpha = true;
        float timeLeft = 0.3f;
        var lerpPct = 1f - Mathf.Exp((Mathf.Log(0.01f) / timeLeft) * Time.deltaTime);
        Color nextColor = new Color(1f, 1f, 1f, bordersImage.color.a + 0.2f);

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            bordersImage.color = Color.Lerp(bordersImage.color, nextColor, lerpPct);

            yield return new WaitForSeconds(0.01f);
        }
        isIncreasingBorderAlpha = false;

    }

    public void ShowActivity() {
        StartCoroutine(ShowActivityCoroutine());
    }
    IEnumerator ShowActivityCoroutine() {
        animator.SetTrigger("showMapTrigger");
        yield return new WaitForSeconds(0.5f);
        isToggled = true;
        marker.gameObject.SetActive(true);
        introCard.SetActive(false);
    }

    public void WinActivity() {
        // play correct sound
        source.PlayOneShot(correct);

        isToggled = false;
        //StartCoroutine(LerpToCorrectPoint());
        marker.anchoredPosition = correctPos;
        StartCoroutine(WinActvityCoroutine());
    }

    IEnumerator WinActvityCoroutine() {
        animator.SetTrigger("finishActivityTrigger");
        informationPanel2.SetActive(true);
        mapTexts.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        informationPanel1.SetActive(false);
    }


    IEnumerator LerpToCorrectPoint()
    {
        float timeLeft = 0.5f;
        var lerpPct = 1f - Mathf.Exp((Mathf.Log(0.01f) / timeLeft) * Time.deltaTime);
        Color red = new Color(0.91f, 0.23f, 0.23f);

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            marker.anchoredPosition = Vector2.Lerp(marker.anchoredPosition, correctPos, lerpPct);
            markerImage.color = Color.Lerp(markerImage.color, red, lerpPct);
            yield return new WaitForSeconds(0.01f);
        }
    }
}