using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

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

            return FindObjectOfType<GameController>().GetComponent<GameController>();
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
    public GameObject playerCamera;
    public GameObject playerMapView;

    GameObject currActivity;

    private CharacterController characterController;
    private FirstPersonController firstPersonController;

    public UnityEvent switchToMapViewEvent;
    public UnityEvent returnToFPSEvent;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        CurrentCamera = Camera.main;
        characterController = playerObj.GetComponent<CharacterController>();
        firstPersonController = playerObj.GetComponent<FirstPersonController>();

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

        playerCamera.SetActive(false);
        playerUI.SetActive(false);
        StereonetUI.SetActive(false);
   
        switchToMapViewEvent.Invoke();
        ToolManager.instance.DisableActiveTool();
    }

    public void ReturnToFPS() {
        currActivity.SetActive(false);
        playerMapView.SetActive(false);
        playerUI.SetActive(true);
        playerCamera.SetActive(true);
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
        characterController.enabled = true;
        firstPersonController.enabled = true;
    }

    public void DisablePlayer()
    {
        characterController.enabled = false;
        firstPersonController.enabled = false;
    }

    public bool IsPlayerEnabled()
    {
        return characterController.enabled;
    }
}

