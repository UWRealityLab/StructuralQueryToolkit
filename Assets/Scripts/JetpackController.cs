using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class JetpackController : MonoBehaviour
{
    public static JetpackController instance;

    private Transform playerTransform;
    float currYVelocity = 0f;

    private FPSController firstPersonController;

    private bool hasStopped = true;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        playerTransform = GameController.instance.playerObj.transform;
        firstPersonController = playerTransform.GetComponent<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStopped && CheckCanMove(currYVelocity))
        { // Slowly lerp the player up/down unless they're blocked
            currYVelocity = Mathf.Lerp(currYVelocity, 0, 0.2f);
            playerTransform.position += new Vector3(0, currYVelocity, 0);
        }
    }

    public void Move(float yVelocity)
    {
        if (!GameController.instance.IsPlayerEnabled())
        {
            return;
        }

        if (!firstPersonController.isGravityOn && CheckCanMove(yVelocity))
        {
            currYVelocity = Mathf.Lerp(currYVelocity, yVelocity, 5f * Time.deltaTime);
            playerTransform.position += new Vector3(0, currYVelocity, 0);
            hasStopped = false;
        }
        else if (CheckCanMove(currYVelocity))
        { // Slowly lerp the player up/down unless they're blocked
            currYVelocity = Mathf.Lerp(currYVelocity, 0, 5f * Time.deltaTime);
            playerTransform.position += new Vector3(0, currYVelocity, 0);
        }
        else
        {
            currYVelocity = 0;
            hasStopped = true;
        }
    }

    bool CheckCanMove(float yDist)
    {

        RaycastHit hit;
        if (!Physics.SphereCast(playerTransform.position, 0.5f, yDist > 0 ? Vector3.up : Vector3.down, out hit, Mathf.Abs(yDist)))
        {
            return true;
        }
        return false;
    }

}
