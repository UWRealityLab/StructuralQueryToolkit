using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneTwoPointerSlider : MonoBehaviour
{
    public static PlaneTwoPointerSlider instance;

    public Transform plane;
    public PiPlotPlane piPlotPlane;
    [SerializeField] Slider slider;
    private Animator aniamtor;

    private float prevSliderValue = 0f;

    private void Start()
    {
        instance = this;
        slider.onValueChanged.AddListener(delegate { UpdateRotation(); });
        aniamtor = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (ToolManager.instance)
        {
            ToolManager.instance.undoEvent.AddListener(Hide);
            ToolManager.instance.switchToolEvent.AddListener(Hide);
        }
    }

    public void UpdateValues(Transform worldPlane, PiPlotPlane piPlotPlane)
    {
        gameObject.SetActive(true);
        plane = worldPlane;
        this.piPlotPlane = piPlotPlane;

        // Set slider values
        prevSliderValue = 0f;
        slider.value = 0f; // temp

        aniamtor.SetBool("isToggled", true);

    }

    public void ConfirmMeasurement()
    {
        gameObject.SetActive(false);
        aniamtor.SetBool("isToggled", false);
    }

    public void CancelMeasurement()
    {
        // This assumes that the 2-point measurement will be the latest measurement the player makes
        gameObject.SetActive(false);
        StereonetsController.singleton.Undo();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        ToolManager.instance.undoEvent.RemoveListener(Hide);
        ToolManager.instance.switchToolEvent.RemoveListener(Hide);
    }

    private void UpdateRotation()
    {
        float deltaAngle = slider.value - prevSliderValue;
        prevSliderValue = slider.value;

        plane.Rotate(Vector3.up, deltaAngle);
        //piPlotPlane.Rotate(piPlotPlane.up, deltaAngle);
        piPlotPlane.SetForward(plane.forward);
        LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane.strike, piPlotPlane.dip);
    }
}
