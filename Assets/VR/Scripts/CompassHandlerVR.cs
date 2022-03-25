using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CompassHandlerVR : MonoBehaviour {

    // Update is called once per frame
    void Update(){
        ChangeToNorth();
    }

    public void ChangeToNorth() {
        transform.rotation = Quaternion.Euler(0, Input.compass.magneticHeading, 0);
        transform.localRotation = Quaternion.Euler(0, -transform.localRotation.eulerAngles.y, 0);
    }
}
