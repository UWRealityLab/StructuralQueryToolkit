using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI samplingRadiusText;

    [SerializeField] Sprite onIcon;
    [SerializeField] Sprite offIcon;

    private bool isAudioEnabled = false;

    [Header("Bootstrap")]
    [SerializeField] TMP_InputField samplingRadiusInput;
    [SerializeField] Image toggleSamplingRadiusImg; 

    private void Start()
    {
        if (Settings.instance.randomSamplingRadius)
        {
            toggleSamplingRadiusImg.sprite = onIcon;
        }
        else
        {
            toggleSamplingRadiusImg.sprite = offIcon;
        }

        samplingRadiusInput.text = Settings.instance.samplingRadius.ToString();
    }

    public void ToggleAudio(Image buttonImage)
    {
        isAudioEnabled = !isAudioEnabled;

        if (isAudioEnabled)
        {
            AudioListener.pause = true;
            buttonImage.sprite = onIcon;
        } else
        {
            AudioListener.pause = false;
            buttonImage.sprite = offIcon;
        }
    }

    public void ToggleSamplingRadius(Image buttonImage)
    {
        Settings.instance.randomSamplingRadius = !Settings.instance.randomSamplingRadius;

        if (Settings.instance.randomSamplingRadius)
        {
            buttonImage.sprite = onIcon;
        }
        else
        {
            buttonImage.sprite = offIcon;
        }
    }

    public void ChangeSamplingRadius(TMP_InputField input)
    {
        float radius;
        if (float.TryParse(input.text, out radius))
        {
            Settings.instance.samplingRadius = radius;
        }
        
    }
}
