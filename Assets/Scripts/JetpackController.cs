using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class JetpackController : MonoBehaviour
{
    public static JetpackController instance;

    float _currYVelocity = 0f;
    public float YVelocity => _currYVelocity;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Move(float yVelocity)
    {
        _currYVelocity = Mathf.Lerp(_currYVelocity, yVelocity, 5f * Time.deltaTime);
    }
}
