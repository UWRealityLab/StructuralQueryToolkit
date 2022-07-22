using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Flipbook : MonoBehaviour
{

    [SerializeField, Range(0.1f, 10f)] private float _delay;
    [SerializeField] private Texture2D[] _images;

    private int _imageIdx = 0;
    private RawImage _image; 
    private Coroutine _flipCo;

    private void Start()
    {
        _image = GetComponent<RawImage>();
    }

    private void OnEnable()
    {
        _flipCo = StartCoroutine(FlipbookCo());
    }

    private void OnDisable()
    {
        StopCoroutine(_flipCo);
        _flipCo = null;
    }

    private IEnumerator FlipbookCo()
    {
        var timeLeft = _delay;
        
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft < 0f)
            {
                _imageIdx = (_imageIdx + 1) % _images.Length;
                _image.texture = _images[_imageIdx];
                timeLeft = _delay;
            }

            yield return null;
        }
    }
}
