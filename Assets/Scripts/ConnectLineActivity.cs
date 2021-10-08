using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConnectLineActivity : MonoBehaviour
{
    [SerializeField]
    Camera activityCamera;
    [SerializeField]
    int pointsToSucceed = 3;
    [SerializeField] GameObject introCard;
    [SerializeField] MeshRenderer answerMesh; // the mesh that will reveal when the activity is done
    [SerializeField] GameObject description;  // Description to appear after successful completion of activity
    [SerializeField] GameObject arrows; 

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip correct;
    [SerializeField] AudioClip incorrect;

    PointCollider[] pointColliders;
    int currPoints;
    Touch touch;
    LineRenderer lineRenderer;

    // UI 
    Animator canvasAnimator;

    bool hasWon = false;

    private void Awake() {
        pointColliders = transform.GetComponentsInChildren<PointCollider>();
        canvasAnimator = transform.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        description.SetActive(false);
        lineRenderer = GetComponent<LineRenderer>();
        for (int i = 0; i < pointColliders.Length; i++) {
            pointColliders[i].index = i;
        }
    }

    private void OnEnable() {
        canvasAnimator.SetTrigger("startActivityTrigger");
    }

    public void HideInfoCard() {
        introCard.SetActive(false);
    }


    public void SetTrigger(string str) {
        canvasAnimator.SetTrigger(str);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasWon) {
            return;
        }

        if (Input.touchCount > 0) {
            touch = Input.GetTouch(0);
        }

        if (Input.GetMouseButton(0) || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
            checkForHit();
        }

        // Check win condition
        // Restart line rendering and points
        if (Input.GetMouseButtonUp(0)) {
            if (currPoints >= pointsToSucceed) {
                // Checking to see if they are adjacent colliders
                // Assumes the collider's ordering is representative by position in array
                int firstOcc = 0;
                while (!(pointColliders[firstOcc].selected)) {  // find first instance of selected collider
                    firstOcc++;
                }
                for(int i = firstOcc; i < firstOcc + pointsToSucceed; i++) {
                    // found a gap in colliders, done checking. Exit this attempt...
                    if (!pointColliders[i].selected) {
                        // reset variables
                        currPoints = 0;
                        for (int j = 0; j < pointColliders.Length; j++) {
                            pointColliders[i].selected = false;
                        }
                        lineRenderer.positionCount = 0;
    
                        return;
                    }
                }
                
                // User drew pointsToSucced number of adjacent colliders, prompt winning visual
                WinActivity();
            }
            currPoints = 0;
            for (int i = 0; i < pointColliders.Length; i++) {
                pointColliders[i].selected = false;
            }
            lineRenderer.positionCount = 0; 
        }
    }

    // 
    void checkForHit() {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(activityCamera.ScreenPointToRay(Input.mousePosition));
        foreach (RaycastHit hit in hits) {
            if (hit.transform.tag.Equals("Point Collider")) {
                PointCollider pointCollider = hit.transform.GetComponent<PointCollider>();
                if (!pointCollider.selected) {
                    currPoints++;
                    pointCollider.selected = true;
                }
                //Debug.Log("Points: " + currPoints);
            }
            if (hit.transform.tag.Equals("Terrain")) {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, Vector3.MoveTowards(hit.point, activityCamera.transform.position, 10f));
            }
        }
    }

    void WinActivity() {
        // play correct sound
        source.PlayOneShot(correct);

        answerMesh.enabled = true;
        lineRenderer.enabled = false;
        hasWon = true;

        // TODO turn on historical canvas text & images
        StartCoroutine(Wait());
    }
    
    // Wait x seconds so the outline appears, then the description
    IEnumerator Wait() {
        yield return new WaitForSeconds(0.5f);
        description.SetActive(true); // Because of the CardUI.cs script, the first card need to warm up before animating
        arrows.SetActive(true);
        canvasAnimator.SetTrigger("startDescriptionTextTrigger");
        yield return new WaitForSeconds(0.5f);
        // To prevent bugginess involved with having multiple animator controllers, we have to disable the parent's animator
        // and enable the child (the first card)
        canvasAnimator.enabled = false; 
    }
}
