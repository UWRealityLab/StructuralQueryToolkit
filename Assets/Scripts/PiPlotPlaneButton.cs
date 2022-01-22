using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class PiPlotPlaneButton : MonoBehaviour
{
    public static PiPlotPlaneButton instance;


    Image image;

    [SerializeField] Color onColor;
    [SerializeField] Color offColor;

    bool canEnablePiPlot = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
    }

    private void Start()
    {
        image.color = offColor;
    }

    public void CombinePlanes()
    {
        if (canEnablePiPlot)
        {
            canEnablePiPlot = false;
            StereonetsController.instance.currStereonet.CalculateAveragePlane();
            image.color = offColor;
        }
    }

    // An event would be better
    public void UpdateButton()
    {
        if (StereonetsController.instance.currStereonet.GetNumPlanes() > 1)
        {
            canEnablePiPlot = true;
            image.color = onColor;
        }
        else
        {
            canEnablePiPlot = false;
            image.color = offColor;
        }
    }

}
