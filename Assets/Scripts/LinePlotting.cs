using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinePlotting : PlayerTool
{
    public static LinePlotting instance;

    [SerializeField] GameObject linePointPrefab;
    [SerializeField] AudioSource flagPlantAudio;
    [SerializeField] GameObject averageButtuon;

    private void Awake()
    {
        instance = this;
    }
    
    public override void Toggle()
    {
        base.Toggle();
        
        MeasureButtonLinear.instance.SetColor(isToggled);
    }

    public override void Enable()
    {
        base.Enable();

        MeasureButtonLinear.instance.SetColor(true);
        averageButtuon.SetActive(true);
        LatestMeasurementUI.instance.ShowLinearMeasurement();
    }

    public override void Disable()
    {
        base.Disable();
        
        MeasureButtonLinear.instance.SetColor(false);
        averageButtuon.SetActive(false);
    }
    
    public override void Undo()
    {
        StereonetsController.instance.UndoLine();
        StereonetCamera.instance.UpdateStereonet();
    }


    public override void UseTool(RaycastHit hit)
    {
        var stereonet = PolePlotting.instance.stereonet;

        flagPlantAudio.Play();

        Transform linePoint = Instantiate(linePointPrefab, hit.point, Quaternion.identity, stereonet.pointPlanesParent).transform;
        linePoint.localScale *= Settings.instance.ObjectScaleMultiplier;
        
        //linePoint.position += hit.normal * 0.1f;
        linePoint.up = hit.normal; // This is not needed if the linePointPrefab is just a sphere
        stereonet.AddLinePoint(linePoint);
        
        StereonetCamera.instance.UpdateStereonet();
    }
}
