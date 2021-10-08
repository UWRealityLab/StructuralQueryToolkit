using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{

    public Transform player;

    private void Start()
    {
        if (player == null)
        {
            player = ToolManager.instance.transform; // lol
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(player.position);
        transform.forward = -transform.forward;
    }
}
