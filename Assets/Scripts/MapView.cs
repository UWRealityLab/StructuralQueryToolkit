using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapView : MonoBehaviour
{
    public static MapView instance;
    public Camera mapViewCamera;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private Canvas mapViewCanvas;
    [SerializeField] private GameObject ResetButton;

    [Header("Dragging")] 
    public float PanSensitivity;

    [Header("Zooming")] 
    public float ZoomSensitivity;
    
    // Zoom increment with the +/- keys
    private float zoomIncrement;

    private Vector3 defaultCamPos;
    private float defaultCamSize;
    
    private bool isInMapView = false;

    private void Awake()
    {
        instance = this;
        defaultCamPos = mapViewCamera.transform.position;
        defaultCamSize = mapViewCamera.orthographicSize;

        zoomIncrement = ZoomSensitivity;
    }

    private void Start()
    {
        playerSprite.localScale *= Settings.instance.ObjectScaleMultiplier;
        GameController.instance.switchToMapViewEvent.AddListener(() =>
        {
            mapViewCamera.gameObject.SetActive(true);
            mapViewCanvas.gameObject.SetActive(true);
            GameController.CurrentCamera = mapViewCamera;
            isInMapView = true;
        });
        GameController.instance.returnToFPSEvent.AddListener(() =>
        {
            mapViewCamera.gameObject.SetActive(false);
            mapViewCanvas.gameObject.SetActive(false);
            GameController.CurrentCamera = Camera.main;
            isInMapView = false;
        });

    }

    private void Update()
    {
        if (isInMapView)
        {
            var isDragging = (Input.GetMouseButton(2)) || (!SketchTool.instance.isToggled && Input.GetMouseButton(0));
            if (isDragging)
            {
                // Dragging sensivity scales with how zoomed in the player is relative to the default zoom
                var zoomPercentage = mapViewCamera.orthographicSize / defaultCamSize;
                var mouseDelta = isDragging ? new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * (PanSensitivity * zoomPercentage) : Vector2.zero;
                mapViewCamera.transform.position += new Vector3(mouseDelta.x, 0f, mouseDelta.y);
            }

            float zoomIncrement = 0f;
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                zoomIncrement = -this.zoomIncrement;
            } else if (Input.GetKeyDown(KeyCode.Minus))
            {
                zoomIncrement = this.zoomIncrement;

            }
            mapViewCamera.orthographicSize = Mathf.Clamp((mapViewCamera.orthographicSize - Input.mouseScrollDelta.y * ZoomSensitivity) + zoomIncrement, 0.1f, defaultCamSize);

            if (!ResetButton.activeSelf)
            {
                ResetButton.SetActive(Math.Abs(mapViewCamera.orthographicSize - defaultCamSize) > float.Epsilon || mapViewCamera.transform.position != defaultCamPos);
            }
        }
    }

    public void GoToMapView()
    {
        GameController.instance.SwitchToMapView(gameObject);
    }

    public void ReturnToDefaultCameraPosition()
    {
        mapViewCamera.transform.position = defaultCamPos;
        mapViewCamera.orthographicSize = defaultCamSize;
    }
}