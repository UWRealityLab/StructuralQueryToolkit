using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.VFX;

public class RockPopupUI : PopupUIShower
{
    private static readonly int _colorProperty = Shader.PropertyToID("_Color");
    private static readonly int _showTrigger = Animator.StringToHash("showTrigger");
    private static readonly int _exitTrigger = Animator.StringToHash("exitTrigger");

    [SerializeField] private Image _bg;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Animator _rockHammerAnimator;
    [SerializeField] private ParticleSystem _hitVFX;

    [Header("Designer Settings")]
    [SerializeField] private GameObject _rockModelPrefab;

    [SerializeField, Tooltip("Replaces the object's current pivot point to be the mean of all of its vertices")] 
    private bool _autoCalculatePivot = true;
    
    [SerializeField] private float _zoomSensitivity = 0.5f;
    [SerializeField] private float _defaultDistanceToCamera = 8f;
    [SerializeField, Tooltip("Max distance the away is from the object")] private float _closestDistToCamera = 100f;
    [SerializeField, Tooltip("Min distance the camera is from the object")] private float _farthestDistFromCamera = 0.1f;

    [TextArea]
    public string Title;

    [TextArea]
    public string Description;

    private Mesh _rockMesh;
    private Material _rockMat;
    private Vector3 _objectCenter;

    protected override void Start()
    {
        base.Start();

        OnPopupShow.AddListener(Show);
    }

    private void Update()
    {
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f)
        {
            Zoom(Input.mouseScrollDelta.y * _zoomSensitivity);
        }
    }

    private void OnValidate()
    {
        Assert.IsNotNull(_rockModelPrefab, "Please insert a rock model in RockPopupUI");
        
        var meshes = _rockModelPrefab.GetComponentsInChildren<MeshFilter>();
        _titleText.text = Title;
        _descriptionText.text = Description;

        if (_autoCalculatePivot)
        {
            _objectCenter = GetMeshCenter();
        }
        else
        {
            _objectCenter = Vector3.zero;
        }
        Vector3 GetMeshCenter()
        {
            var center = Vector3.zero;
            var numVertices = 0;
            foreach (var mesh in meshes)
            {
                var vertices = mesh.sharedMesh.vertices;
                foreach (var vertex in vertices)
                {
                    center += vertex;
                }
                numVertices += vertices.Length;
            }
            
            center /= numVertices;
            return center;
        }
    }

    public void SetBackgroundColor(Image img)
    {
        _bg.color = img.color;
    }

    public void ResetCameraRotation()
    {
        RockPopupManager.Instance.ResetCameraRotation();
    }

    public void ZoomIncrement()
    {
        RockPopupManager.Instance.Zoom(_zoomSensitivity, _closestDistToCamera, _farthestDistFromCamera);
    }

    public void ZoomDecrement()
    {
        RockPopupManager.Instance.Zoom(-_zoomSensitivity, _closestDistToCamera, _farthestDistFromCamera);
    }
    
    public void Zoom(float amount)
    {
        RockPopupManager.Instance.Zoom(amount, _closestDistToCamera, _farthestDistFromCamera);
    }

    protected override void Show()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ShowCoroutine());
        }

        RockPopupManager.Instance.SetRockModel(_rockModelPrefab, _rockModelPrefab.transform.rotation, _objectCenter, _defaultDistanceToCamera);
    }

    IEnumerator ShowCoroutine()
    {
        _rockHammerAnimator.SetTrigger(_showTrigger);
        isTransitioning = true;

        yield return new WaitForSeconds(1f);
        //_hitVFX.Play();
        mat.SetColor(_colorProperty, Color.blue);
        base.Show();
    }

    public override void Exit()
    {
        _rockHammerAnimator.SetTrigger(_exitTrigger);

        RockPopupManager.Instance.ResetZoom();
        base.Exit();
    }
    
}
