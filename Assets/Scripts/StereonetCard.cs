using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StereonetCard : MonoBehaviour, IDashboardCard
{
    [SerializeField] RawImage stereonetImage;
    [SerializeField] RectTransform stereonetImage2D;
    [SerializeField] Image descriptionCardImage;
    public TMP_InputField titleInputField;
    [SerializeField] public EventTrigger fullscreenEventTrigger;
    [SerializeField] Image fullscreenImage;
    [SerializeField] Image editImage;
    [SerializeField] Image colorImage;
    [SerializeField] Image selectedIconImage;
    [SerializeField] TMP_Text idText;

    private void Awake()
    {
        
    }

    public void SetID(int id)
    {
        idText.text = $"#{id}";
    }

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
        stereonetImage2D.gameObject.SetActive(false);
        
        stereonetImage.texture = newImage;
    }

    public void SetStereonet2DImage(Stereonet2D stereonet)
    {
        stereonetImage.enabled = false;
        
        stereonet.MoveStereonetUI(stereonetImage2D);
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
        StereonetDashboard.instance.OpenSwatch();
    }
}
