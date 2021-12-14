using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PIPlotButton : MonoBehaviour
{
    public static PIPlotButton instance;

    Image image;

    [SerializeField] Color onColor;
    [SerializeField] Color offColor;
    [SerializeField] Color disabledColor;

    bool canEnablePiPlot = false;
    public bool isToggled = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
    }

    private void Start()
    {
        image.color = disabledColor;
    }

    public void TogglePIPlot()
    {
        if (canEnablePiPlot)
        {
            isToggled = !isToggled;

            PolePlotting.instance.stereonet.SetLineRenderer(isToggled);
            StereonetsController.instance.currStereonet.isPiPlotEnabled = isToggled;

            if (isToggled)
            {
                image.color = onColor;
            }
            else
            {
                image.color = offColor;
            }
            
            StereonetCamera.instance.UpdateStereonet();
        }
    }

    // An event would be better
    public void UpdateButton()
    {
        // lol
        if (PolePlotting.instance.stereonet.GetNumPoints() >= 3)
        {
            canEnablePiPlot = true;
            if (isToggled)
            {
                image.color = onColor;
            }
            else
            {
                image.color = offColor;
            }
        }
        else
        {
            canEnablePiPlot = false;
            image.color = disabledColor;
        }
    }
}
