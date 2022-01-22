using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiPlotLinearButton : MonoBehaviour
{
    public static PiPlotLinearButton instance;

    Image image;

    [SerializeField] Color onColor;
    [SerializeField] Color disabledColor;

    private bool canToggleButton;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        image = GetComponent<Image>();
        image.color = disabledColor;
    }


    public void CombineLines()
    {
        if (canToggleButton)
        {
            canToggleButton = false;
            image.color = disabledColor;

            StereonetsController.instance.currStereonet.CalculateAverageLine();
        }
    }


    // An event would be better
    public void UpdateButton()
    {
        // lol
        if (StereonetsController.instance.currStereonet.GetNumLines() > 1)
        {
            canToggleButton = true;
            image.color = onColor;
        }
        else
        {
            canToggleButton = false;
            image.color = disabledColor;
        }
    }
}
