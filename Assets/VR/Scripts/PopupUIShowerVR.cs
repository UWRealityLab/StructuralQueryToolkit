using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PopupUIShowerVR : MonoBehaviour
{
    private static int outlineShaderID;

    [SerializeField] private MeshRenderer RockHammerMeshRenderer;
    [SerializeField] GameObject UICanvas;
    [SerializeField] Animator UIAnimator;

    private float outlineAmount = 1f;
    private static float outlineMin = 1f;
    private static float outlineMax = 1.5f;
    private static float outlineScale = 1.5f;
    
    private bool isTransitioning = false;
    private bool isMouseOver = false;
    
    private Material mat;
    private static readonly int _showTrigger = Animator.StringToHash("showTrigger");
    private static readonly int _exitTrigger = Animator.StringToHash("exitTrigger");
    
    private XRSimpleInteractable _xrSimpleInteractable;

    private void Awake()
    {
        outlineShaderID = Shader.PropertyToID("_Outline");
        mat = RockHammerMeshRenderer.sharedMaterial;
        mat.SetFloat(outlineShaderID, outlineAmount);
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            UICanvas.SetActive(false);
        }
        
        _xrSimpleInteractable.hoverExited.AddListener(hoverEventArgs =>
        {
            OnMouseExit();
        });
        
        _xrSimpleInteractable.selectEntered.AddListener(args =>
        {
            OnMouseUp();
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
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnMouseUp()
    {
        if (!isTransitioning && !UICanvas.activeSelf)
        {
            StartCoroutine(ShowCoroutine());
        }
    }
    IEnumerator ShowCoroutine()
    {
        UICanvas.SetActive(true);
        UIAnimator.SetTrigger(_showTrigger);
        isTransitioning = true;
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }

    public void Exit() => StartCoroutine(ExitCoroutine());
    IEnumerator ExitCoroutine()
    {
        UIAnimator.SetTrigger(_exitTrigger);
        yield return new WaitForSeconds(0.5f);
        UICanvas.SetActive(false);
    }


}
