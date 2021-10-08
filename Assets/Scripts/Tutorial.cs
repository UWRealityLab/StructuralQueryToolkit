using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using UnityEditor;

public class Tutorial : MonoBehaviour, INotificationReceiver
{
    public static Tutorial instance;
    public static double frameTime = 1d / 60d;

    public bool canSeekForward = false; // Whether seek is enabled or not (usually enabled when the timeline is paused)
    public bool canSeekBackward = false;

    [SerializeField] Transform playerTransform;

    PlayableDirector tutorialDirector;

    [SerializeField] List<GameObject> disableAtStartObjects;

    // All the pause markers in the timeline
    // Indicate to pause the timeline at frame perfect times
    private List<PauseMarker> pauseMarkers;

    // Number of continues the timeline can go through
    // This will allow the timeline to continue past a pause marker even if it
    // hasn't reached a pause marker yet
    [SerializeField] int continueCount = 0;

    [SerializeField] private bool isPaused = true;

    private int currPauseMarkerID = 0;


    private void Awake()
    {
        tutorialDirector = GetComponent<PlayableDirector>();
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize all notifications in timeline
        var timeline = (TimelineAsset)tutorialDirector.playableAsset;
        var markers = timeline.markerTrack.GetMarkers().ToList();

        pauseMarkers = new List<PauseMarker>(markers.Count);

        for (int i = 0; i < markers.Count; i++)
        {
            var pauseMarker = (PauseMarker)markers[i];
            //pauseMarker.id = new PropertyName(i);
            //print(pauseMarker.time);
            //pauseMarkers[i] = pauseMarker;
            pauseMarkers.Add(pauseMarker);
        }

        pauseMarkers.Sort();

        for (int i = 0; i < pauseMarkers.Count; i++)
        {
            pauseMarkers[i].id = new PropertyName(i);
        }

        foreach (var go in disableAtStartObjects)
        {
            go.SetActive(false);
        }
        StartCoroutine(ContinueCoroutine());
        
    }

    int pointsToContinue = -1;
    public void SetPointsToContinue(int points) {
        pointsToContinue = points;
    }

    // Update is called once per frame
    void Update()
    {
        if (pointsToContinue > 0 && Input.GetMouseButtonUp(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200f)) {
                if (hit.transform.CompareTag("Tutorial Tap Point") &&  ToolManager.instance.activeTool.GetType() == typeof(PolePlotting) ) {

                    hit.transform.gameObject.SetActive(false);

                    pointsToContinue -= 1;
                    if (pointsToContinue == 0) {
                        Continue();
                    }

                    // Since we blocked the one raycast from CompassActivity, we manaully call another
                    PolePlotting.instance.CheckCanUseTool();
                }
            }
        }
    }

    /// <summary>
    /// Attempts to fix timeline from seeking past a pause signal (due to framerate)
    /// </summary>
    /// <param name="time"></param>
    public void Pause()
    {
        //tutorialDirector.Pause();
        print("Pause");
    }

    public void Continue()
    {
        continueCount++;
    }

    // Because a player can continue faster than the timeline reaching a Pause Marker,
    // we need to only continue when the pause marker is reached
    private IEnumerator ContinueCoroutine()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (continueCount <= 0 || !isPaused)
            {
                continue;
            }

            isPaused = false;
            continueCount--;
            tutorialDirector.Resume();
            tutorialDirector.Play();
            tutorialDirector.extrapolationMode = DirectorWrapMode.Hold;

            TutorialUI.instance.DisableSeekButtons();
        }

    }


    public void Continue(float nextMarkerTime) {
        tutorialDirector.extrapolationMode = DirectorWrapMode.Hold;
        tutorialDirector.playableGraph.GetRootPlayable(0).SetDuration(nextMarkerTime);
        tutorialDirector.Resume();
        continueCount++;

        TutorialUI.instance.DisableSeekButtons();
    }

    public void SeekForward()
    {
        if (canSeekForward)
        {
            canSeekForward = false;
            TutorialUI.instance.DisableSeekButtons();
            //tutorialDirector.Resume();
            Continue();
        }

    }

    public void SeekBackward(float time)
    {
        if (canSeekBackward)
        {
            canSeekBackward = false;
            TutorialUI.instance.DisableSeekButtons();
            //TutorialUI.instance.SetCanSeekForward(true);
            //tutorialDirector.playableGraph.GetRootPlayable(0).SetDuration(pauseMarkers[currPauseMarkerID - 1].time);// Make the maximum time the current time
            //currPauseMarkerID--;
            tutorialDirector.time -= time;
            tutorialDirector.Resume();
        }
    }

    public void SetCanSeekForward(bool canSeek)
    {
        canSeekForward = canSeek;
        TutorialUI.instance.SetCanSeekForward(canSeek);
    }

    public void SetCanSeekBackward(bool canSeek)
    {
        canSeekBackward = canSeek;
        TutorialUI.instance.SetCanSeekBackward(canSeek);
    }

    public void JumpTo(float time) {

        TutorialUI.instance.DisableSeekButtons();
        tutorialDirector.time = time;

        // Because pause marker are retroactive, we must force the timeline to
        // continue after some number of frames has passed
        StartCoroutine(JumpToCoroutine());
    }

    IEnumerator JumpToCoroutine()
    {
        yield return new WaitForEndOfFrame();
        tutorialDirector.Resume();
    }

    public void MakeGameObjectMeasurable(GameObject obj)
    {
        obj.tag = "Terrain";
    }


    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification != null)
        {
            //double time = origin.IsValid() ? origin.GetTime() : 0.0;
            //Debug.LogFormat("Received notification of type {0} at time {1}", notification.GetType(), time);

            // This notification type needs to be of a Pause Marker
            if (notification.GetType() != typeof(PauseMarker))
            {
                return;
            }

            // Pause timeline, and set the next max duration to the next pause marker (this prevents timeline from overjumping from low framerates)
            tutorialDirector.Pause();
            int pauseMarkerID = notification.id.GetHashCode();
            currPauseMarkerID = pauseMarkerID;
            isPaused = true;

            // 2nd condition is that the next marker's time MUST be larger than the current one (this allows jumping forward in the timeline to not cause retroactive pausemarkers from
            // truncating the timeline duration, thus pausing the timeline when it shouldn't have)
            if (pauseMarkerID < pauseMarkers.Count - 1)
            {
                tutorialDirector.playableGraph.GetRootPlayable(0).SetDuration(pauseMarkers[pauseMarkerID + 1].time);
            } else
            {
                print("reached the end of the tutorial");
            }

            //print(pauseMarkers[pauseMarkerID + 1].time);
        }
    }

    public void FreezePlayer()
    {

    }

    public void MovePlayer(Transform otherTransform)
    {
        playerTransform.position = otherTransform.position;
        playerTransform.rotation = otherTransform.rotation;
    }

    public void TurnOffAllTutorialArrows()
    {
        var proceedTriggerObjects = Resources.FindObjectsOfTypeAll(typeof(ProceedButtonTrigger));
        foreach (var obj in proceedTriggerObjects)
        {
            Destroy(obj);
        }

        var arrows = Resources.FindObjectsOfTypeAll(typeof(TutorialArrow));
        foreach (var obj in arrows)
        {
            var arrow = (TutorialArrow)obj;
            Destroy(arrow.gameObject);
        }

    }

}
