using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Used in animation events
    /// </summary>
    public void DestroyGameobject()
    {
        if (Application.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }

    public void ToggleBool(string boolName) {
        animator.SetBool(boolName, !animator.GetBool(boolName));
    }

    // Redudant function, but Unity's button component system can only take in one argumment
    public void SetBoolFalse(string boolName) {
        animator.SetBool(boolName, false);
    }
    public void SetBoolTrue(string boolName) {
        animator.SetBool(boolName, true);
    }
}
