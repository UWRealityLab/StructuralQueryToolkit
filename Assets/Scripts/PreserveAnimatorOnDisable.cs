using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// IMPORTANT: place this component on TOP of your animator component in the
/// inspector menu
/// </summary>
public class PreserveAnimatorOnDisable : MonoBehaviour
{

    private class AnimParam
    {
        public AnimatorControllerParameterType type;
        public string paramName;
        object data;

        public AnimParam(Animator animator, string paramName, AnimatorControllerParameterType type)
        {
            this.type = type;
            this.paramName = paramName;
            switch (type)
            {
                case AnimatorControllerParameterType.Int:
                    this.data = (int)animator.GetInteger(paramName);
                    break;
                case AnimatorControllerParameterType.Float:
                    this.data = (float)animator.GetFloat(paramName);
                    break;
                case AnimatorControllerParameterType.Bool:
                    this.data = (bool)animator.GetBool(paramName);
                    break;
            }
        }
        public object getData()
        {
            return data;
        }

        public void UpdateData(Animator animator)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Int:
                    this.data = (int)animator.GetInteger(paramName);
                    break;
                case AnimatorControllerParameterType.Float:
                    this.data = (float)animator.GetFloat(paramName);
                    break;
                case AnimatorControllerParameterType.Bool:
                    this.data = (bool)animator.GetBool(paramName);
                    break;
            }
        }
    }

    Animator animator;
    List<AnimParam> parms = new List<AnimParam>();

    [Tooltip("Parameters with these names will not be preserved")]
    public List<string> parameterExceptions;
 
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.keepAnimatorControllerStateOnDisable = true;
        for (int i = 0; i < animator.parameters.Length; i++)
        {
            AnimatorControllerParameter p = animator.parameters[i];
            if (parameterExceptions.Contains(p.name))
            {
                continue;
            }
            AnimParam ap = new AnimParam(animator, p.name, p.type);
            parms.Add(ap);
        }
    }

    public void OnDisable()
    {
        for(int i = 0; i < parms.Count; i++)
        {
            parms[i].UpdateData(animator);
        }
    }

    void OnEnable()
    {
        foreach (AnimParam p in parms)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(p.paramName, (int)p.getData());
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(p.paramName, (float)p.getData());
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(p.paramName, (bool)p.getData());
                    break;
            }
        }
        parms.Clear();
    }
}
