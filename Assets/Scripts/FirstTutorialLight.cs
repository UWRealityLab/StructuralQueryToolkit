using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTutorialLight : MonoBehaviour
{
    [SerializeField] float blendDistance;
    [SerializeField] Material mat;

    private Transform cameraMain;

    private void Start()
    {
        cameraMain = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, cameraMain.position);
        float intensity = Mathf.Min(dist / blendDistance, 1f) * 3f;
        //print(intensity);

        mat.SetVector("_EmissionColor", new Vector4(1.5f, 1.17f, 0.97f, 1) * intensity); 
    }
}
