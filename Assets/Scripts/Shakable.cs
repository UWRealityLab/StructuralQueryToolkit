using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shakable : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private float _scale = 0.5f;

    private float seed;

    private void Awake()
    {
        seed = Random.value;   
    }

    public void Shake()
    {
        cameraTransform.localPosition += new Vector3(
            Mathf.PerlinNoise(0, Time.time * _frequency) * 2 - 1,
            Mathf.PerlinNoise(1, Time.time * _frequency) * 2 - 1,
            Mathf.PerlinNoise(2, Time.time * _frequency) * 2 - 1
        ) * _scale;
    }

    public void Reset()
    {
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, Vector3.zero, 0.2f);
    }
}
