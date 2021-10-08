using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorSwatchButton : MonoBehaviour
{

    private Color color;
    [SerializeField] Material flagMaterial;
    [SerializeField] Material twoPointPlaneMaterial;

    private void Start()
    {
        color = GetComponent<Image>().color;
    }

    public void UpdateStereonetColor()
    {
        StereonetDashboard.singleton.selectedCard.GetComponent<StereonetCard>().SetColor(color);
        StereonetsController.singleton.currStereonet.ChangeFlagsMaterial(flagMaterial, twoPointPlaneMaterial);

        StereonetDashboard.singleton.CloseSwatch();
    }
}
