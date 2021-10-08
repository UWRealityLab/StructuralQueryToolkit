using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AlignToNormal : MonoBehaviour
{

    public bool isFlippedNormal = false;

    // Sends a raycast through this transform's backward vector, and faces at
    // the direction of the hit's normal
    public void AlignWithRaycast()
    {
        RaycastHit hit;

        Vector3 dir = isFlippedNormal ? transform.forward : -transform.forward;

        if (!Physics.Raycast(transform.position, dir, out hit, 200f))
        {
            Debug.Log("Align raycast found no colliders");
            return;
        }
        

        transform.forward = isFlippedNormal ? -hit.normal : hit.normal;

    }
}


