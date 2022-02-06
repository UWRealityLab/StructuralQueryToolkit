using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PolePlotting : PlayerTool
{
    public static PolePlotting instance;

    public static PolePlotting Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PolePlotting>();
            }

            return instance;
        }
    }

    [SerializeField] public bool attachFlagsToObject = false;
    [SerializeField] GameObject flagPrefab;
    [SerializeField] GameObject piPlotButton;

    [SerializeField] AudioSource flagPlantAudio;
    
    
    private void Awake()
    {
        instance = this;
    }
    
    public override void Toggle()
    {
        base.Toggle();
        
        MeasureButtonPole.instance.SetColor(isToggled);
    }

    public override void Enable()
    {
        base.Enable();
        
        MeasureButtonPole.instance.SetColor(true);
        piPlotButton.SetActive(true);
        LatestMeasurementUI.instance.ShowPoleMeasurement();
    }

    public override void Disable()
    {
        base.Disable();
        
        MeasureButtonPole.instance.SetColor(false);
        piPlotButton.SetActive(false);
    }
    
    // Get a random # of raycasts around a top point in order to get a mean vector
    public override void UseTool(RaycastHit hit)
    {
        List<Vector3> currNormals = new List<Vector3>();
        float elevation = hit.point.y;
        currNormals.Add(hit.normal);

        var currStereonet = StereonetsController.instance.currStereonet;

        Transform flag = Instantiate(flagPrefab, hit.point, Quaternion.identity).transform;
        if (!attachFlagsToObject)
        {
            currStereonet.AddPoleFlag(flag);
        }
        else
        {
            currStereonet.AddPoleFlag(flag, hit.transform);
        }
        flag.localScale = new Vector3(flag.localScale.x, flag.localScale.x, flag.localScale.x) * Settings.instance.ObjectScaleMultiplier;
        
        flag.up = hit.normal;
        var flagComponent = flag.transform.GetComponent<Flag>();
        flagComponent.stereonet = currStereonet;
        
        // Was a valid spot to place a flag, so play audio
        flagPlantAudio.Play();

        // SAMPLING RADIUS
        if (Settings.instance.randomSamplingRadius)
        {
            Ray cameraRay = GameController.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 localRandVector3;
            RaycastHit randomHit;
            for (int i = 0; i < Settings.instance.maxSampleCount; i++)
            {
                localRandVector3 = new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y) * Settings.instance.samplingRadius;

                //if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition + randVector3), out hit))
                //{
                //    continue;
                //}

                Vector3 newPoint = transform.TransformPoint(transform.InverseTransformPoint(hit.point - cameraRay.direction) + localRandVector3);

                if (!Physics.Raycast(newPoint, cameraRay.direction, out randomHit, 5f))
                {
                    continue;
                }

                if (randomHit.transform.tag.Equals("Terrain"))
                {
                    currNormals.Add(randomHit.normal);
                    
                    #if UNITY_EDITOR
                    Debug.DrawRay(randomHit.point, randomHit.normal, Color.white, 3f);
                    #endif
                }
            }
        }

        // If any raycasts were successful
        if (currNormals.Count > 0)
        {
            Vector3 meanVector = CalculateMeanVector(currNormals);
            StereonetUtils.CalculateStrikeAndDip(meanVector, out float strike, out float dip);
            StereonetUtils.CalculateTrendAndPlunge(meanVector, out float trend, out float plunge);
            UpdateLatestMeasurementUI(strike, dip, elevation);
            currStereonet.AddStrikeAndDip(strike, dip);
            currStereonet.AddTrendPlunge(trend, plunge);

            if (Settings.instance.showElevationData)
            {
                currStereonet.AddPole(meanVector, elevation, flagComponent);
            }
            else
            {
                currStereonet.AddPole(meanVector, flagComponent);
            }

            if (PIPlotButton.instance != null)
            {
                PIPlotButton.instance.UpdateButton();
            }
        }
        
        StereonetCamera.instance.UpdateStereonet();
    }

    Vector3 CalculateMeanVector(List<Vector3> normals)
    {
        Vector3 meanVector = Vector3.zero;
        foreach (Vector3 normal in normals)
        {
            meanVector += normal;
        }
        meanVector /= normals.Count;

        return meanVector;

    }

    public override void Undo()
    {
        StereonetsController.instance.UndoPole();
        StereonetCamera.instance.UpdateStereonet();
    }

    public void ClearMeasurements()
    {
        StereonetsController.instance.currStereonet.ClearPoles();
    }

    // Calculates the strike and dip of the current measurement's mean vector
    private void UpdateLatestMeasurementUI(float strike, float dip, float elevation)
    {
        LatestMeasurementUI.instance.SetStrikeDipInformation(strike, dip, elevation);
    }
}

