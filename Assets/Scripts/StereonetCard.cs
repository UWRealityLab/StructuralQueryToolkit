using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StereonetCard : MonoBehaviour
{
    [SerializeField] RawImage stereonetImage;
    [SerializeField] Image descriptionCardImage;
    [SerializeField] TMP_InputField titleInputField;
    [SerializeField] public EventTrigger fullscreenEventTrigger;
    [SerializeField] Image fullscreenImage;
    [SerializeField] Image editImage;
    [SerializeField] Image colorImage;
    [SerializeField] Image selectedIconImage;

    public RawImage GetImage()
    {
        return stereonetImage;
    }

    public string GetTitle()
    {
        return titleInputField.text;
    }

    public void SetStereonetImage(Texture newImage)
    {
        stereonetImage.texture = newImage;
    }

    public void SetColor(Color color)
    {
        descriptionCardImage.color = color;
    }

    public Texture GetStereonetTexture()
    {
        return stereonetImage.texture;
    }

    public void Rename()
    {
        titleInputField.ActivateInputField();
    }

    public void Select()
    {
        fullscreenImage.enabled = true;
        editImage.enabled = true;
        colorImage.enabled = true;
        selectedIconImage.enabled = true;
        
        titleInputField.ActivateInputField();
    }

    public void Deselect()
    {
        fullscreenImage.enabled = false;
        editImage.enabled = false;
        colorImage.enabled = false;
        selectedIconImage.enabled = false;

    }

    public void OpenColorSwatch()
    {
        StereonetDashboard.singleton.OpenSwatch();
    }
}
