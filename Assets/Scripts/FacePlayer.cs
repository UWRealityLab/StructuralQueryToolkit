using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public bool flip = false;
    public Transform player;

    private static readonly float LookAtFrameRate = 30f;

    private void Start()
    {
        if (player == null)
        {
            player = Camera.main.transform; // lol
        }
        
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(LookAtPlayer), 0f, 1f / LookAtFrameRate);
    }

    private void OnDisable()
    {
        CancelInvoke((nameof(LookAtPlayer)));
    }

    private void LookAtPlayer()
    {
        transform.LookAt(player.position);
        transform.forward = flip ? -transform.forward : transform.forward;
    }

    public void ChangePlayer(Transform trans)
    {
        player = trans;
    }
}

