using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public bool flip = false;
    public Transform player;

    private void Start()
    {
        if (player == null)
        {
            player = ToolManager.instance.transform; // lol
        }
        
        //InvokeRepeating(nameof(LookAtPlayer), 0f, 0.025f);
    }

    private void FixedUpdate()
    {
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        transform.LookAt(player.position);

        if (flip)
        {
            transform.forward = -transform.forward;
        }
    }

    public void ChangePlayer(Transform trans)
    {
        player = trans;
    }
}

