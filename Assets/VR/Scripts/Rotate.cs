using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float RotateSpeed = 15f;
    
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, RotateSpeed * Time.deltaTime);
    }
}
