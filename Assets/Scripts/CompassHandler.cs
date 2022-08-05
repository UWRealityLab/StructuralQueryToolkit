using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassHandler : MonoBehaviour {
    public Vector3 North;
    public Transform Player;

    public RectTransform NorthLayer;

    // Update is called once per frame
    void Update(){
        ChangeToNorth();
    }

    public void ChangeToNorth() {
        North.z = Player.eulerAngles.y;
        NorthLayer.localEulerAngles = North;
    }

}


