using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRotationBinder : MonoBehaviour
{
    public bool flipValues = false;
    public Transform[] transforms;

    [SerializeField] Slider slider;

    // Update is called once per frame
    void Update()
    {
        if (flipValues)
        {
            foreach (var trans in transforms)
            {
                trans.localRotation = Quaternion.Euler(-slider.value, trans.localRotation.y, trans.localRotation.z);
            }
        }
        else
        {
            foreach (var trans in transforms)
            {
                trans.localRotation = Quaternion.Euler(slider.value, trans.localRotation.y, trans.localRotation.z);
            }
        }

    }
}
