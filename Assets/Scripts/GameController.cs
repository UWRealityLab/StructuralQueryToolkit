using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class GameController : MonoBehaviour
{
    public Texture2D cursor;

    private static GameController _instance;
    public static GameController instance
    {
        get
        {
            if (_instance)
            {
                return _instance;
            }

            _instance = FindObjectOfType<GameController>();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    // The current used camera (either the player camera or the map camera)
    public static Camera CurrentCamera;

    // Define cameras and canvases
    public GameObject playerObj;
    public GameObject playerUI;
    public GameObject StereonetUI;
    public GameObject playerMapView;

    GameObject currActivity;


    public UnityEvent switchToMapViewEvent;
    public UnityEvent returnToFPSEvent;

    public bool IsVR;

    private void Awake() {
        instance = this;
        playerMapView = playerObj.GetComponentInChildren<SpriteRenderer>(true).gameObject;
    }

    private void Start() {
        CurrentCamera = Camera.main;
        var test = FindObjectOfType<XROrigin>(true);
        IsVR = test != null;
        //print(IsVR);

        if (cursor)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }
    }

    public void SwitchToActivity(GameObject activity) {
        currActivity = activity;
        playerObj.SetActive(false);
        playerUI.SetActive(false);
        StereonetUI.SetActive(false);
        activity.SetActive(true);
    }

    // Need a separate one to not disable the player UI (so we can see our figure on the map)
    public void SwitchToMapView(GameObject activity)
    {
        currActivity = activity;
        activity.SetActive(true);
        playerMapView.SetActive(true);

        playerUI.SetActive(false);
        StereonetUI.SetActive(false);
   
        switchToMapViewEvent.Invoke();
        ToolManager.instance.DisableActiveTool();
    }

    public void ReturnToFPS() {
        currActivity.SetActive(false);
        playerMapView.SetActive(false);
        playerUI.SetActive(true);
        StereonetUI.SetActive(true);
        returnToFPSEvent.Invoke();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Quit()
    {
        Application.Quit();
    }


    public void EnablePlayer()
    {
    }

    public void DisablePlayer()
    {
    }

    public bool IsPlayerEnabled()
    {
        return false;
    }
}

