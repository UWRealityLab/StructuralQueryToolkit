using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The only reason this class exists is to save the parameter of the button pressed 
// because when its disabled, the parameters revert to its default state
public class ToggleButtonsController : MonoBehaviour
{
    [SerializeField] string toggleParamName;
    Animator animator;

    [SerializeField] bool savedParamState = false;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void UpdateSavedParamState()
    {
        savedParamState = animator.GetBool(toggleParamName);
    }

    private void OnEnable()
    {
        animator.SetBool(toggleParamName, savedParamState);
    }

}
