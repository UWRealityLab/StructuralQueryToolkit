using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasureButtonPlane : MonoBehaviour
{
    public static MeasureButtonPlane instance;

    Image image;

    [SerializeField] Color onColor;
    [SerializeField] Color offColor;

    private void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
        SetColor(false);
    }

    public void SetColor(bool isEnable)
    {
        if (isEnable)
        {
            image.color = onColor;
        }
        else
        {
            image.color = offColor;
        }
    }
}
