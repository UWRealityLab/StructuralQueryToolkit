using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FreeMoveLocomotion : MonoBehaviour
{
    [SerializeField] private Volume _postProcessingVolume;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _cameraTransform;
    
    private bool _isGrounded = true;
    private bool _isMoving = false;
    
    [Header("Vignette Effect")] 
    public float FadeTime = 0.5f;
    [Range(0, 1f)]
    public float Intensity = 1f;

    private float _currFadeTime = 0f;
    private Vignette _vignette;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _postProcessingVolume.profile.TryGet(out _vignette);
    }

    private void Update()
    {
        var dist = Settings.instance.JetpackMovementSpeed * Time.deltaTime;
        var canMove = CanMove(transform.forward, dist);
        if (_isMoving && canMove)
        {
            _playerTransform.position += transform.forward * dist;
        }
    }

    private bool CanMove(Vector3 dir, float dist)
    {
        Assert.IsTrue(dist > 0);
        Debug.DrawLine(_cameraTransform.position, _cameraTransform.position + dir * dist, Color.cyan);
        return !Physics.Raycast(_cameraTransform.position, dir.normalized, out var hit, dist);
    }

    public void StartMoving()
    {
        _isMoving = true;
        if (enabled)
        {
            FadeIn();
        }
    }

    public void StopMoving()
    {
        _isMoving = false;
        FadeOut();
    }

    private void FadeIn()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(Fade(Intensity));
    }

    private void FadeOut()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float endValue)
    {
        var elapsedTime = _currFadeTime - FadeTime;
        var startValue = _vignette.intensity.value;

        while (elapsedTime < FadeTime)
        {
            elapsedTime += Time.deltaTime;
            _currFadeTime = elapsedTime;

            var currIntensity = Mathf.Lerp(startValue, endValue, elapsedTime / FadeTime);
            _vignette.intensity.Override(currIntensity);

            yield return null;
        }
        
        _currFadeTime = FadeTime;
        _vignette.intensity.Override(endValue);

        _fadeCoroutine = null;
    }

    private void OnDisable()
    {
        ResetVignette();
    }

    private void OnDestroy()
    {
        ResetVignette();
    }

    private void ResetVignette()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _currFadeTime = 0f;
        _vignette.intensity.Override(0f);
    }
}
