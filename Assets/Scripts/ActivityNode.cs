using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityNode : MonoBehaviour
{

    public GameObject activity;
    public AudioSource source;
    public AudioClip sound;
    public GameObject mapCamera;

    private void OnMouseUp() {
        // Switch to activity by enabling it

        // Compass activity must be turned off
        // No UI elements can be pressed
        // Dashboard must be off
        if (!StereonetDashboard.singleton.gameObject.activeSelf) { 
            source.PlayOneShot(sound);

            //activity.SetActive(true);

            // This is a current bug where users can tap the nodes from map view.
            // To deal with it for now, I turn off the map camera so they can go to the activity
            mapCamera.SetActive(false);
            GameController.instance.SwitchToActivity(activity);
        }
    }

}
