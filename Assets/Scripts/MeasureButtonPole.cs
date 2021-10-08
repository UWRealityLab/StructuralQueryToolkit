using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//
public class MeasureButtonPole : MonoBehaviour
{
    public static MeasureButtonPole instance;

    Image image;

    [SerializeField] Color onColor;
    [SerializeField] Color offColor;

    private void Start()
    {
        instance = this;
        image = GetComponent<Image>();
        image.color = offColor;
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
