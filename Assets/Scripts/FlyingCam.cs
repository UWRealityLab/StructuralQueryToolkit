using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCam : MonoBehaviour {

    private Vector3 _angles;
    public float speed = 5.0f;
    public float fastSpeed = 2.0f;
    public float mouseSpeed = 4.0f;
    // changed mouse
    public Transform target;


    private void OnEnable()
    // it was from here that I got the code that is now placed in the word doc. that code was preventing the mouse from being on.

    {
        _angles = transform.eulerAngles;
   

    }

   

    private void Update()
    {
        _angles.x -= Input.GetAxis("Mouse Y") * mouseSpeed;
        _angles.y += Input.GetAxis("Mouse X") * mouseSpeed;
        transform.eulerAngles = _angles;
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : speed;
        transform.position +=
            Input.GetAxis("Horizontal") * moveSpeed * transform.right +
            Input.GetAxis("Vertical") * moveSpeed * transform.forward;


        // added to first script to 
        transform.LookAt(target);
    }
}