using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapView : MonoBehaviour
{
    public static MapView instance;
    public Camera mapViewCamera;
    private Transform playerSprite;
    public int CameraFrameRate = 30;
    [SerializeField] private Canvas mapViewCanvas;
    private RectTransform mapViewCanvasRectTrans;
    [SerializeField] private GameObject ResetButton;
    

    [Header("Dragging")] 
    public float PanSensitivity;

    public bool _flipDragging = false;

    [Header("Zooming")] 
    public float ZoomSensitivity;
    
    // Zoom increment with the +/- keys
    private float zoomIncrement;

    private Vector3 defaultCamPos;
    private float defaultCamSize;
    private bool isInMapView = false;
    public bool IsInMapView => isInMapView;

    [Header("Diagrams in Map View")] 
    [SerializeField] private GameObject StereonetMarkerPrefab;
    [SerializeField] private Transform StrikeDipIconsParent;
    private List<StereonetMapMarker> _strikeDipMarkers;
    private float _markerFontSize = 36;

    private Vector2 _uiOffset;

    private void Awake()
    {
        instance = this;
        defaultCamPos = mapViewCamera.transform.position;
        defaultCamSize = mapViewCamera.orthographicSize;

        mapViewCanvasRectTrans = mapViewCanvas.GetComponent<RectTransform>();

        _strikeDipMarkers = new List<StereonetMapMarker>();
        
        _uiOffset = new Vector2(mapViewCanvasRectTrans.sizeDelta.x / 2f, mapViewCanvasRectTrans.sizeDelta.y / 2f);

        zoomIncrement = ZoomSensitivity;
    }

    private void Start()
    {
        playerSprite = GameController.instance.playerMapView.transform;
        playerSprite.localScale *= Settings.instance.ObjectScaleMultiplier;
        GameController.instance.switchToMapViewEvent.AddListener(() =>
        {
            mapViewCanvas.gameObject.SetActive(true);
            GameController.CurrentCamera = mapViewCamera;
            mapViewCamera.enabled = true;
            isInMapView = true;
            InvokeRepeating(nameof(RenderMapCamera), 0f, 1f / CameraFrameRate);
        });
        GameController.instance.returnToFPSEvent.AddListener(() =>
        {
            mapViewCanvas.gameObject.SetActive(false);
            GameController.CurrentCamera = Camera.main;
            mapViewCamera.enabled = false;
            isInMapView = false;
            GameController.instance.EnablePlayer();
            CancelInvoke(nameof(RenderMapCamera));
        });

        if (GameController.instance.IsVR)
        {
            StereonetsController.instance.OnStereonetCreated.AddListener(stereonet =>
            {
                var marker = Instantiate(StereonetMarkerPrefab, StrikeDipIconsParent).GetComponent<StereonetMapMarker>();
                marker.SetFontSize(_markerFontSize);
                marker.SetPosition(new Vector2(float.MaxValue, float.MaxValue));
                marker.SetId(stereonet.id);
                _strikeDipMarkers.Add(marker);
            
                stereonet.OnStereonetUpdate.AddListener(stereonet =>
                {
                    var uiPos = mapViewCamera.WorldToViewportPoint(stereonet.CalculateCentroid());
                    var canvasPos = new Vector2(uiPos.x * mapViewCanvasRectTrans.sizeDelta.x, uiPos.y * mapViewCanvasRectTrans.sizeDelta.y);
                    marker.SetPosition(canvasPos - _uiOffset);
                });
            
                stereonet.OnStereonetDestroy.AddListener(stereonet =>
                {
                    if (marker)
                    {
                        Destroy(marker.gameObject);
                    }
                });
            });
        }
        
        //gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isInMapView)
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount == 2)
                {
                    PinchToZoom();
                }

                if (Input.touchCount > 0)
                {
                    TouchDragging();
                }
            }
            else
            {
                var isDragging = (Input.GetMouseButton(2)) || (SketchTool.instance && !SketchTool.instance.isToggled && Input.GetMouseButton(0));
                if (isDragging)
                {
                    // Drag sensitivity scales with how zoomed in the player is relative to the default zoom
                    var zoomPercentage = mapViewCamera.orthographicSize / defaultCamSize;
                    var mouseDelta = isDragging ? new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * (PanSensitivity * zoomPercentage) : Vector2.zero;
                    mouseDelta = _flipDragging ? -mouseDelta : mouseDelta;
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
            }

            if (!ResetButton.activeSelf)
            {
                ResetButton.SetActive(Math.Abs(mapViewCamera.orthographicSize - defaultCamSize) > float.Epsilon || mapViewCamera.transform.position != defaultCamPos);
            }
        }

    }

    private void RenderMapCamera()
    {
        mapViewCamera.Render();
    }

    private const float MOBILE_MODIFIER = 0.1f;
    private float pinchInitialDist;
    private void PinchToZoom()
    {
        
        var pos1 = Input.GetTouch(0).position;
        var pos2 = Input.GetTouch(1).position;

        var delta = Vector2.Distance(pos1, pos2);

        if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
        {
            pinchInitialDist = delta;
        }
        else
        {
            var pinchAmount = delta - pinchInitialDist;
            mapViewCamera.orthographicSize = Mathf.Clamp((mapViewCamera.orthographicSize - pinchAmount * ZoomSensitivity * MOBILE_MODIFIER), 0.1f, defaultCamSize);
        }
        
        pinchInitialDist = delta;
        
    }

    private Vector2 prevDragPos;
    private void TouchDragging()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            prevDragPos = Input.GetTouch(0).position;
            return;
        }

        if (Input.touchCount == 2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                print("A");
                prevDragPos = Input.GetTouch(1).position;
                return;
            }

            if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                print("B");
                prevDragPos = Input.GetTouch(0).position;
                return;
            }

        }
        
        var pos1 = Input.GetTouch(0).position;
        //var pos2 = Input.GetTouch(1).position;

        var posDelta = prevDragPos - pos1;
        posDelta = _flipDragging ? -posDelta : posDelta;

        var zoomPercentage = mapViewCamera.orthographicSize / defaultCamSize;
        var mouseDelta = new Vector2(posDelta.x, posDelta.y) * (PanSensitivity * zoomPercentage) * MOBILE_MODIFIER;
        mapViewCamera.transform.position += new Vector3(mouseDelta.x, 0f, mouseDelta.y);
        
        prevDragPos = pos1;
    }

    public void GoToMapView()
    {
        GameController.instance.SwitchToMapView(gameObject);
    }

    public void ExitMapView()
    {
        GameController.instance.ReturnToFPS();
        gameObject.SetActive(true); // TODO 
    }

    public void ToggleMapView()
    {
        if (gameObject.activeSelf)
        {
            GameController.instance.ReturnToFPS();
        }
        else
        {
            GameController.instance.SwitchToMapView(gameObject);
        }
    }

    public void ReturnToDefaultCameraPosition()
    {
        mapViewCamera.transform.position = defaultCamPos;
        mapViewCamera.orthographicSize = defaultCamSize;
    }

    public void SetMarkerSize(float size)
    {
        _markerFontSize = size;

        foreach (var marker in _strikeDipMarkers)
        {
            marker.SetFontSize(_markerFontSize);
        }
    }
}