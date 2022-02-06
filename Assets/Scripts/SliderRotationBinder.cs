using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class SliderRotationBinder : MonoBehaviour
{
    private enum RotationAxis
    {
        X,
        Y,
        Z,
        Custom
    }

    [SerializeField] private RotationAxis rotationAxis;

    public bool flipRotation = false;
    [SerializeField] Transform[] transforms;
    private DynamicTerrain[] _dynModels;

    [Header("Misc")] 
    [SerializeField] private Transform customRotationAxisTransform;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.onValueChanged.AddListener((val) =>
        {
            RotateTransforms();
        });
    }
    
    private void RotateTransforms()
    {
        Vector3 rotateAxis;
        var rotateAmount = flipRotation ? -slider.value : slider.value;
        
        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotateAxis = Vector3.right;
                break;
            case RotationAxis.Y:
                rotateAxis = Vector3.up;
                break;
            case RotationAxis.Z:
                rotateAxis = Vector3.forward;
                break;
            case RotationAxis.Custom:
                rotateAxis = customRotationAxisTransform.forward;
                break;
            default:
                rotateAxis = Vector3.zero;
                break;
        }
        
        foreach (var trans in transforms)
        {
            trans.localRotation = Quaternion.AngleAxis(rotateAmount, rotateAxis);
        }

        foreach (var dynModel in _dynModels)
        {
            dynModel.UpdateStereonet();
        }
    }

    private void OnValidate()
    {
        customRotationAxisTransform.gameObject.SetActive(rotationAxis == RotationAxis.Custom);
        
        _dynModels = new DynamicTerrain[transforms.Length];

        for (int i = 0; i < transforms.Length; i++)
        {
            var trans = transforms[i];
            if (!trans.TryGetComponent<DynamicTerrain>(out var dynTerrain))
            {
                trans.gameObject.AddComponent<DynamicTerrain>();
            }

            _dynModels[i] = dynTerrain;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 rotateAxis;
        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotateAxis = Vector3.right;
                break;
            case RotationAxis.Y:
                rotateAxis = Vector3.up;
                break;
            case RotationAxis.Z:
                rotateAxis = Vector3.forward;
                break;
            case RotationAxis.Custom:
                rotateAxis = customRotationAxisTransform.forward;
                break;
            default:
                rotateAxis = Vector3.zero;
                break;
        }

        foreach (var trans in transforms)
        {
            Debug.DrawLine(trans.position - rotateAxis, trans.position + rotateAxis, Color.cyan, 0f, false);
        }
    }
}
