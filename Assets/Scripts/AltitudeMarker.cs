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
    [HideInInspector] public Vector3 pos;

    private bool isDirty = false;

    private static bool hasUpdatedSettings = false;
    
    private void Awake()
    {
        pos = transform.position;
    }

    private void Start()
    {
        pos = transform.position;
        UpdateAltitudeBias();
        Destroy(gameObject);
    }

    private void Update()
    {
        if (transform.hasChanged && isDirty)
        {
            transform.hasChanged = false;
            pos = transform.position; 
            UpdateAltitudeBias();
        }
    }

    private void LateUpdate()
    {
        pos = transform.position;
        UpdateAltitudeBias();
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        isDirty = true;
        //UpdateAltitudeBias();
    }

    public static void UpdateAltitudeBias()
    {
        if (hasUpdatedSettings)
        {
            return;
        }

        hasUpdatedSettings = true;
        
        var avgAltitude = 0f;
        
        var altitudeMarkers = FindObjectsOfType<AltitudeMarker>(true);
        foreach (var currAltitudeMarker in altitudeMarkers)
        {
            avgAltitude += currAltitudeMarker.elevation - currAltitudeMarker.pos.y;
        }
        
        avgAltitude /= altitudeMarkers.Length;
        
        var settings = FindObjectOfType<Settings>().GetComponent<Settings>();
        settings.elevationBias = avgAltitude;
        
        //print($"Altitude Bias: {avgAltitude}");
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, 0.25f);
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