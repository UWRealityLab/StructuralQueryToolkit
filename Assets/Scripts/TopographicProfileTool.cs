using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityStandardAssets.Characters.ThirdPerson;

public class TopographicProfileTool : PlayerTool
{
    public static TopographicProfileTool instance;

    public static TopographicProfileTool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TopographicProfileTool>();
            }

            return instance;
        }
    }

    
    [SerializeField] private GameObject topographicProfileMeasurementPrefab;
    [SerializeField] private GameObject profileMarkerPrefab;
    [SerializeField] private Transform ProfileLockPlane;
    [SerializeField] private LineRenderer ProfileLockLineRenderer;

    public int sigDigits;
    
    [Header("On screen UI")]
    [SerializeField] private UILineRenderer lineRendererUI;
    [SerializeField] private TextMeshProUGUI topLeftCornerText;
    [SerializeField] private TextMeshProUGUI bottomRightCornerText;
    [SerializeField] private SegmentedControlButton profileLockButton;
    [SerializeField] private SegmentedControlButton visibilityButton;
    [SerializeField] private ButtonsDrawer buttonsDrawer;

    private List<TopographicProfileMeasurement> _profiles;
    private TopographicProfileMeasurement _activeProfile;

    private Material _profilePlaneMat;
    private const float MARKER_ALPHA = 0.5f;
    private static readonly int _colorShaderProperty = Shader.PropertyToID("_Color");
    private const float PROFILE_LOCK_DIST_THRESHOLD = 0.5f; // Minimum distance the profile marker needs to be from the profile plane to be valid 

    private Color _currColor;
    private float _currMarkerSize;

    private bool _isValidMarkerSpot = false;
    private bool _isGloballyVisible = true;
    private static readonly int _baseColorShaderProperty = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        instance = this;
        _profiles = new List<TopographicProfileMeasurement>();
        transform.position = Vector3.zero;
    }

    protected override void Start()
    {
        base.Start();

        _currMarkerSize = 1f;
        _currColor = buttonsDrawer.GetActiveButtonColor();
        _currColor.a = MARKER_ALPHA;
        if (Application.isPlaying)
        {
            _profilePlaneMat = ProfileLockPlane.GetComponentInChildren<MeshRenderer>(true).material;
            var planeColor = new Color(_currColor.r, _currColor.g, _currColor.b, 0.25f);
            _profilePlaneMat.SetColor(_baseColorShaderProperty, planeColor);
        }
        ProfileLockLineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (isToggled && _isGloballyVisible && _activeProfile && _activeProfile.IsProfileLocked && _activeProfile.markerPoints.Count >= 2)
        {
            // Show preview of mouse cursor to profile plane
            var cameraRay = GameController.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(cameraRay, out RaycastHit hit) || !hit.collider.tag.Equals("Terrain"))
            {
                return;
            }
            GetClosestPointToPlane(hit.point, out var closestPlanePos, out var dist);

            ProfileLockLineRenderer.positionCount = 2;
            ProfileLockLineRenderer.SetPosition(0, hit.point);
            ProfileLockLineRenderer.SetPosition(1, closestPlanePos);

            _isValidMarkerSpot = dist < PROFILE_LOCK_DIST_THRESHOLD;
            
            var color = _isValidMarkerSpot ? Color.green : Color.red;
            ProfileLockLineRenderer.material.SetColor(_colorShaderProperty, color);
        }
        else
        {
            _isValidMarkerSpot = false;
        }
    }

    public override void Enable()
    {
        base.Enable();
        
        if (_activeProfile.markerPoints.Count >= 2 && _activeProfile.IsProfileLocked && _isGloballyVisible)
        {
            ShowProfilePlane();
        }
    }

    public override void Disable()
    {
        base.Disable();
        HideProfilePlane();
    }

    private void GetClosestPointToPlane(Vector3 point, out Vector3 closestPoint, out float dist)
    {
        var planeNormal = ProfileLockPlane.right;
        var pointInPlane = _activeProfile.PlanePos; // Get any point that is inside the plane
        var vec = point - pointInPlane;
        dist = Vector3.Dot(vec, planeNormal);
        
        closestPoint = point - planeNormal * dist;
        dist = Mathf.Abs(dist);
    }

    public override void UseTool(RaycastHit hit)
    {
        if (!_isGloballyVisible)
        {
            _isGloballyVisible = true;
            visibilityButton.SetState(true);
            return;
        }
        
        if (_isValidMarkerSpot || _activeProfile.markerPoints.Count < 2 || !_activeProfile.IsProfileLocked)
        {
            PlaceProfileMarker(hit);
        }
        
        if (_activeProfile.markerPoints.Count == 2 && _activeProfile.IsProfileLocked)
        {
            // The first 2 marker points can be placed anywhere, but once placed, the profile lock will force subsequent
            // profile markers to be placed aligning to the profile plane
            ShowProfilePlane();
            UpdateProfilePlane();
        }
    }

    private void UpdateProfilePlane()
    {
        var latestPos = _activeProfile.markerPoints.First.Value.position;
        var prevPos = _activeProfile.markerPoints.First.Next.Value.position;
        _activeProfile.PlanePos = prevPos;
        _activeProfile.PlaneDir = prevPos - latestPos;
    }

    private void PlaceProfileMarker(RaycastHit hit)
    {
        var rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
        var markerTrans = Instantiate(profileMarkerPrefab, hit.point, rotation, _activeProfile.transform).transform;
        _activeProfile.markerPoints.AddFirst(markerTrans);
        markerTrans.GetComponentInChildren<MeshRenderer>().material.SetColor(_baseColorShaderProperty, _currColor);
        markerTrans.localScale = new float3(_currMarkerSize) * Settings.instance.ObjectScaleMultiplier;
        
        if (_activeProfile.IsProfileLocked && _activeProfile.markerPoints.Count > 2)
        {
            _activeProfile.AddPointProfileLocked(hit);
        }
        else
        {
            _activeProfile.AddPointProfileUnlocked(hit);
        }
        
        UpdateGraph();
    }

    public void SetProfileLockState(bool state)
    {
        if (state)
        {
            if (!_activeProfile.IsProfileLocked && _activeProfile.markerPoints.Count >= 2 && _isGloballyVisible)
            {
                ShowProfilePlane();
            }
        }
        else
        {
            if (_activeProfile.IsProfileLocked)
            {
                HideProfilePlane();
            }
        }
        
        _activeProfile.IsProfileLocked = state;

    }
    
    // Show profile lock plane that aligns to the active profile
    private void ShowProfilePlane()
    {
        ProfileLockPlane.gameObject.SetActive(true);
        ProfileLockPlane.forward = _activeProfile.PlaneDir;
        ProfileLockPlane.position = _activeProfile.PlanePos;

        ProfileLockLineRenderer.positionCount = 2;
    }

    private void HideProfilePlane()
    {
        ProfileLockPlane.gameObject.SetActive(false);
        
        ProfileLockLineRenderer.positionCount = 0;
    }

    public override void Undo()
    {
        _activeProfile.Undo();

        if (_activeProfile.markerPoints.Count < 2 && _activeProfile.IsProfileLocked)
        {
            HideProfilePlane();
            ClearGraph();
        }
        else
        {
            UpdateGraph();
        }
    }

    public void SelectProfile(int index)
    {
        _activeProfile = _profiles[index];
        
        if (!_activeProfile.IsProfileLocked || _activeProfile.markerPoints.Count < 2)
        {
            HideProfilePlane();
            ClearGraph();
        }
        else if (_activeProfile.markerPoints.Count > 1 && _activeProfile.IsProfileLocked && _isGloballyVisible)
        {
            ShowProfilePlane();
        }

        // Update UI elements
        profileLockButton.SetState(_activeProfile.IsProfileLocked);
        UpdateGraph();
    }

    public void DeleteProfile(int index)
    {
        _profiles[index].Destroy();
        _profiles.RemoveAt(index);
    }
    
    public void DeleteActiveProfile()
    {
        _activeProfile.Destroy();
        _profiles.Remove(_activeProfile);
    }

    public void DeleteAll()
    {
        foreach (var profile in _profiles)
        {
            profile.Destroy();
        }

        _profiles.Clear();
    }
    
    public void CreateNewProfile()
    {
        var newProfile = Instantiate(topographicProfileMeasurementPrefab, transform).GetComponent<TopographicProfileMeasurement>();
        _profiles.Add(newProfile);
        _activeProfile = newProfile;
    }

    public void SetProfileVisibility(int index, bool state)
    {
        _profiles[index].SetVisibilityState(state);
    }

    public bool GetProfileVisibility(int index)
    {
        return _profiles[index].GetVisibility();
    }

    public void SetGlobalVisibility(bool state)
    {
        _isGloballyVisible = state;
        
        if (_activeProfile.IsProfileLocked && _activeProfile.markerPoints.Count >= 2)
        {
            if (state)
            {
                ShowProfilePlane();
            }
            else
            {
                HideProfilePlane();
            }
        }
        
        foreach (var profile in _profiles)
        {
            profile.SetGlobalVisibility(state);
        }
    }

    public void SetMarkerSize(Slider slider)
    {
        _currMarkerSize = slider.value;
        foreach (var profile in _profiles)
        {
            profile.SetMarkerSize(_currMarkerSize);
        }
    }

    public void SetColor(Image colorImg)
    {
        var color = colorImg.color;
        _currColor = new Color(color.r, color.g, color.b, MARKER_ALPHA);
        foreach (var profile in _profiles)
        {
            profile.SetColor(_currColor);
        }

        var profilePlaneColor = new Color(color.r, color.g, color.b, 0.25f);
        _profilePlaneMat.SetColor(_baseColorShaderProperty, profilePlaneColor);
    }

    public TopographicProfileMeasurement GetProfile(int profileIndex)
    {
        return _profiles[profileIndex];
    }

    public Vector2[] GetActiveGraphPoints(Vector2 graphDimensions)
    {
        return _activeProfile.GetGraphPoints(graphDimensions);
    }
    
    public Vector2[] GetGraphPoints(int profileIndex, Vector2 graphDimensions)
    {
        return _profiles[profileIndex].GetGraphPoints(graphDimensions);
    }

    public Vector2[] GetActiveRawGraphPoints()
    {
        return _activeProfile.GetRawGraphPoints();
    }
    
    public Vector2[] GetRawGraphPoints(int profileIndex)
    {
        return _profiles[profileIndex].GetRawGraphPoints();
    }
    
    private void UpdateGraph()
    {
        var tempArr = GetActiveGraphPoints(lineRendererUI.rectTransform.rect.size);
        
        lineRendererUI.Points = tempArr;
        if (_activeProfile.markerPoints.Count > 1)
        {
            bottomRightCornerText.text = $"{_activeProfile.TotalDistance.ToString($"F{sigDigits}")}m";
            topLeftCornerText.text = $"{_activeProfile.MaxHeight.ToString($"F{sigDigits}")}m";
        }
        else
        {
            ClearGraph();
        }
    }
    
    private void ClearGraph()
    {
        lineRendererUI.Points = new Vector2[1]; // NOTE: Setting points to an empty Vector2 array creates a visual artifact
        bottomRightCornerText.text = "";
        topLeftCornerText.text = "";
    }
}

