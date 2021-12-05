using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AltitudeMarker : MonoBehaviour
{
    public float elevation = 0f;

    // Apparently, you cannot access transform during OnValidate(), so we'll copy the position
    [HideInInspector] public float yPos;

    private void Awake()
    {
        yPos = transform.position.y;
    }

    private void Start()
    {
        Destroy(gameObject);
    }

    public static void UpdateAltitudeBias()
    {
        var avgAltitude = 0f;
        
        var altitudeMarkers = FindObjectsOfType<AltitudeMarker>(false);
        var settings = FindObjectOfType<Settings>().GetComponent<Settings>();

        if (altitudeMarkers.Length == 0)
        {
            settings.elevationBias = 0f;
            return;
        }
        
        foreach (var currAltitudeMarker in altitudeMarkers)
        {
            avgAltitude += currAltitudeMarker.elevation - currAltitudeMarker.yPos;
        }
        
        avgAltitude /= altitudeMarkers.Length;
        
        settings.elevationBias = avgAltitude;
        
        //print($"Altitude Bias: {avgAltitude}");
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(AltitudeMarker))]
public class AltitudeMarkerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Anchor down"))
        {
            AltitudeMarker altitudeMarker = (AltitudeMarker)target;
            
            if (Physics.Raycast(altitudeMarker.transform.position, Vector3.down, out var raycastHit, 500f))
            {
                altitudeMarker.transform.position = raycastHit.point;
                AltitudeMarker.UpdateAltitudeBias();
            }
            else
            {
                Debug.LogError("Anchor failed: there are no valid colliders below the altitude marker");
            }
        }
    }
}
#endif