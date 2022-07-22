using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class FacePlayerAroundPivot : MonoBehaviour
{
    public bool flip = false;
    public Transform player;
    public Transform Pivot;

    [Space] 
    public float PivotRadius;

    private static readonly float LookAtFrameRate = 30f;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<XROrigin>().transform;
            player = Camera.main.transform;
        }
        
        InvokeRepeating(nameof(LookAtPlayer), 0f, 1f / LookAtFrameRate);
    }

    private void LookAtPlayer()
    {
        if (math.distance(((float3)transform.position).xz, ((float3)player.position).xz) <= PivotRadius)
        {
            return;
        }

        var playerPos = math.round(player.position);
        
        transform.LookAt(playerPos);
        transform.forward = flip ? -transform.forward : transform.forward;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        
        var newPosition = Pivot.position + -transform.forward * PivotRadius;
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
    }

    public void ChangePlayer(Transform trans)
    {
        player = trans;
    }
}

