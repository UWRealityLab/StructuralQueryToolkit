using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialCollider : MonoBehaviour
{

    public UnityEvent collideEvent;

    private void OnTriggerEnter(Collider other)
    {
        collideEvent.Invoke();
        gameObject.SetActive(false);
    }
}
