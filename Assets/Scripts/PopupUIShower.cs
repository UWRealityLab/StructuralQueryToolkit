using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PopupUIShower : MonoBehaviour
{
    private static int outlineShaderID;

    [SerializeField] protected GameObject UICanvas;
    [SerializeField] protected Animator UIAnimator;

    public bool DisablePlayer = true;

    private float outlineAmount = 1f;
    private static float outlineMin = 1f;
    private static float outlineMax = 1.5f;
    private static float outlineScale = 1.5f;
    
    protected bool isTransitioning = false;
    protected bool isMouseOver = false;
    
    protected Material mat;
    private static readonly int _showTrigger = Animator.StringToHash("showTrigger");
    private static readonly int _exitTrigger = Animator.StringToHash("exitTrigger");

    protected UnityEvent OnPopupShow;

    protected virtual void Awake()
    {
        outlineShaderID = Shader.PropertyToID("_Outline");
        mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetFloat(outlineShaderID, outlineAmount);
        OnPopupShow = new UnityEvent();
    }

    protected virtual void Start()
    {
        if (Application.isPlaying)
        {
            UICanvas.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Destroy(mat);
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Mouse is over UI
            return;
        }
        
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
        if (isMouseOver && !isTransitioning && !UICanvas.activeSelf && !MapView.instance.IsInMapView)
        {
            Show();
        }
    }

    protected virtual void Show()
    {
        StartCoroutine(ShowCoroutine());
        OnPopupShow.Invoke();
    }
    IEnumerator ShowCoroutine()
    {
        UICanvas.SetActive(true);
        UIAnimator.SetTrigger(_showTrigger);
        if (DisablePlayer)
        {
            GameController.instance.DisablePlayer();
        }
        isTransitioning = true;
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }

    public virtual void Exit() => StartCoroutine(ExitCoroutine());
    IEnumerator ExitCoroutine()
    {
        UIAnimator.SetTrigger(_exitTrigger);
        if (DisablePlayer)
        {
            GameController.instance.EnablePlayer();
        }
        yield return new WaitForSeconds(0.5f);
        UICanvas.SetActive(false);
    }


}
