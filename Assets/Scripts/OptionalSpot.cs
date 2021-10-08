using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalSpot : MonoBehaviour
{
    [SerializeField] Animator popupAnimator;


    private void OnTriggerEnter(Collider other)
    {
        popupAnimator.gameObject.SetActive(true);
        popupAnimator.SetBool("isToggled", true);
    }

    private void OnTriggerExit(Collider other)
    {
        popupAnimator.SetBool("isToggled", false);
    }
}
