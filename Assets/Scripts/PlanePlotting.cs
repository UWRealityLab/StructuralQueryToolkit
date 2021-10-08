using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanePlotting : PlayerTool
{
    public static PlanePlotting instance;
    public static LayerMask stereonetLayer;

    public enum MeasureMode
    {
        TwoPoint,
        ThreePoint
    }

    public MeasureMode measureMode = MeasureMode.TwoPoint;

    [SerializeField] GameObject planePointPrefab;
    [SerializeField] AudioSource flagPlantAudio;
    [SerializeField] GameObject averageButtuon;
    [SerializeField] GameObject planeToolsWindow;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        stereonetLayer = LayerMask.NameToLayer("Stereonet");
    }

    public override void Toggle()
    {
        base.Toggle();
        
        MeasureButtonPlane.instance.SetColor(isToggled);
    }

    public override void Enable()
    {
        base.Enable();

        MeasureButtonPlane.instance.SetColor(true);
        averageButtuon.SetActive(true);
        planeToolsWindow.SetActive(true);
        LatestMeasurementUI.instance.ShowPlaneMeasurement();
    }

    public override void Disable()
    {
        base.Disable();
        
        MeasureButtonPlane.instance.SetColor(false);
        planeToolsWindow.SetActive(false);
        averageButtuon.SetActive(false);
        PolePlotting.instance.stereonet.ClearUnfinishedPlanePoints();
    }

    public override void Undo()
    {
        StereonetsController.singleton.UndoPlane();
    }

    public override void UseTool(RaycastHit hit)
    {
        var stereonet = PolePlotting.instance.stereonet;

        flagPlantAudio.Play();

        Transform planePoint = Instantiate(planePointPrefab, hit.point, Quaternion.identity, stereonet.pointPlanesParent).transform;
        planePoint.localScale *= Settings.instance.ObjectScaleMultiplier;
        planePoint.up = hit.normal;

        // TODO
        planePoint.transform.GetComponent<MeshRenderer>().material = stereonet.flagMaterials[1];

        if (measureMode == MeasureMode.TwoPoint)
        {
            // If the 2-point slider is active, it implies that the user hasn't confirmed the measurement yet
            // but we will just confirm for them anayways
            if (PlaneTwoPointerSlider.instance.gameObject.activeSelf)
            {
                PlaneTwoPointerSlider.instance.ConfirmMeasurement();
            }

            stereonet.AddPlanePointTwoPoint(planePoint);
        } else
        {
            stereonet.AddPlanePointThreePoint(planePoint);
        }
    }

    public void SetThreePointMeasurement()
    {
        measureMode = MeasureMode.ThreePoint;
        PolePlotting.instance.stereonet.ClearUnfinishedPlanePoints();
        PlaneTwoPointerSlider.instance.ConfirmMeasurement();
    }
    public void SetTwoPointMeasurement()
    {
        measureMode = MeasureMode.TwoPoint;
        PolePlotting.instance.stereonet.ClearUnfinishedPlanePoints();
    }
}
