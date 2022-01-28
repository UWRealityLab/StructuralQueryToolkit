using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveForBrowserApp : MonoBehaviour
{
    void Awake() {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Destroy(gameObject);
        }
    }
}
