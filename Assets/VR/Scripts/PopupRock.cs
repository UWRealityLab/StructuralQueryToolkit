using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class RockPopup
{
    
}

[RequireComponent(typeof(XRSimpleInteractable))]
public class PopupRock : MonoBehaviour
{
    private static int outlineShaderID;

    [SerializeField] private MeshRenderer _hammerMeshRenderer;
    [SerializeField] private Transform _rockTransform;
    [SerializeField] private Animator _rockHammerAnimator;
    [SerializeField] private VisualEffect _hitVFX;
    
    [SerializeField] private GameObject _popupObjects;
    [SerializeField] private Animator _popupUIAnimator;
    [SerializeField] private Transform _rockHitPositionTransform;
    [SerializeField] private Transform _rockPositionTransform;
    
    private float outlineAmount = 1f;
    private static float outlineMin = 1f;
    private static float outlineMax = 2f;
    private static float outlineScale = 1.5f;
    
    private bool isTransitioning = false;
    private bool isMouseOver = false;
    
    private Material mat;
    private static readonly int _showTrigger = Animator.StringToHash("showTrigger");
    private static readonly int _exitTrigger = Animator.StringToHash("exitTrigger");

    private XRSimpleInteractable _xrSimpleInteractable;

    private Vector3 _originalRockScale;

    private void Awake()
    {
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        _originalRockScale = _rockTransform.localScale;
        _rockTransform.gameObject.SetActive(false);
        
        outlineShaderID = Shader.PropertyToID("_Outline");
        mat = _hammerMeshRenderer.material;
        mat.SetFloat(outlineShaderID, outlineAmount);
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            _popupObjects.SetActive(false);
        }
        
        _xrSimpleInteractable.hoverExited.AddListener(hoverEventArgs =>
        {
            OnMouseExit();
        });
        
        _xrSimpleInteractable.selectEntered.AddListener(args =>
        {
            Show();
        });
    }

    private void Update()
    {
        if (_xrSimpleInteractable.isHovered)
        {
            OnMouseOver();
        }
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
#endif
    }

    private void OnMouseOver()
    {
        outlineAmount = Mathf.Clamp(outlineAmount + Time.deltaTime * outlineScale, outlineMin, outlineMax);
        mat.SetFloat(outlineShaderID, outlineAmount);
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        StartCoroutine(RemoveOutlineCoroutine());
    }

    private IEnumerator RemoveOutlineCoroutine()
    {
        while (outlineAmount > float.Epsilon && !isMouseOver)
        {
            outlineAmount = Mathf.Clamp(outlineAmount - Time.deltaTime * outlineScale, outlineMin, outlineMax);
            mat.SetFloat(outlineShaderID, outlineAmount);
            yield return null;
        }
    }
    
    public void ShowPopupObjects()
    {
        _popupObjects.SetActive(true);
        StartCoroutine(ShowRockCoroutine());
        ShowHitVFX();
    }

    private IEnumerator ShowRockCoroutine()
    {
        _rockTransform.gameObject.SetActive(true);
        _rockTransform.position = _rockHitPositionTransform.position;
        _rockTransform.localScale = Vector3.zero;
        var timeLeft = 0.5f;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            _rockTransform.position = Vector3.Lerp(_rockTransform.position, _rockPositionTransform.position, 10f * Time.deltaTime);
            _rockTransform.localScale = Vector3.Lerp(_rockTransform.localScale, _originalRockScale, 10f * Time.deltaTime);
            yield return null;
        }

        _rockTransform.position = _rockPositionTransform.position;
        _rockTransform.localScale = _originalRockScale;
    }
    
    private void Show()
    {
        if (!isTransitioning && !_popupObjects.activeSelf)
        {
            StartCoroutine(ShowCoroutine());
        }
    }
    
    private void ShowHitVFX()
    {
        _hitVFX.Play();
    }
    
    IEnumerator ShowCoroutine()
    {
        _rockHammerAnimator.SetTrigger(_showTrigger);
        isTransitioning = true;
        yield return new WaitForSeconds(0.5f);
        _popupUIAnimator.SetTrigger(_showTrigger);
        
        isTransitioning = false;
    }

    public void Exit() => StartCoroutine(ExitCoroutine());
    IEnumerator ExitCoroutine()
    {
        if (isTransitioning)
        {
            yield break;
        }
        _rockHammerAnimator.SetTrigger(_exitTrigger);
        _popupUIAnimator.SetTrigger(_exitTrigger);
        
        var timeLeft = 0.5f;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            _rockTransform.position = Vector3.Lerp(_rockTransform.position, _rockHitPositionTransform.position, 5f * Time.deltaTime);
            _rockTransform.localScale = Vector3.Lerp(_rockTransform.localScale, Vector3.zero, 5f * Time.deltaTime);
            yield return null;
        }

        _rockTransform.gameObject.SetActive(false);

        _popupObjects.SetActive(false);
    }

}
