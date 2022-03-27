using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class LatestMeasurementUI : MonoBehaviour
{
    public static LatestMeasurementUI instance;

    [SerializeField] TMP_Text infoText;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(TestCo());
    }

    private IEnumerator TestCo()
    {
        var elapsedTime = 0f;
        var cnt = 0;
        while (elapsedTime < 50f)
        {
            elapsedTime += Time.deltaTime;
            //SetText($"{cnt}");
            //print($"{infoText.transform.name}");
            
            cnt++;
            yield return null;
        }

    }

    /// <summary>
    /// Set the latest measurement UI with the given text
    /// </summary>
    public void SetText(string text)
    {
        infoText.SetText(text);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowPoleMeasurement()
    {
        if (Settings.instance.showElevationData)
        {
            infoText.text = "Strike:\nDip:\nElevation:";
        } else
        {
            infoText.text = "Strike:\nDip:";
        }
    }

    public void ShowPlaneMeasurement()
    {
        infoText.text = "Strike:\nDip:";
    }

    public void ShowLinearMeasurement()
    {
        infoText.text = "Trend:\nPlunge:";
    }

    public void SetPlaneMeasurementInformation(float strike, float dip)
    {
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.AppendFormat("Strike: {0}°\nDip: {1}°",
        Mathf.Round(strike).ToString("000"),
        Mathf.Round(dip).ToString("00")
        );

        SetText(strBuilder.ToString());

    }

    public void SetTrendPlungeInformation(float trend, float plunge)
    {
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.AppendFormat("Trend: {0}°\nPlunge: {1}°",
        Mathf.Round(trend).ToString("000"),
        Mathf.Round(plunge).ToString("00")
        );

        SetText(strBuilder.ToString());

    }

    public void SetStrikeDipInformation(float strike, float dip, float elevation)
    {
        StringBuilder strBuilder = new StringBuilder();

        if (Settings.instance.showElevationData)
        {
            strBuilder.AppendFormat("Strike: {0}°\nDip: {1}°\nElevation: {2} m",
            Mathf.Round(strike).ToString("000"),
            Mathf.Round(dip).ToString("00"),
            System.Math.Round(elevation + Settings.instance.elevationBias, 2)
            );
        }
        else
        {
            strBuilder.AppendFormat("Strike: {0}°\nDip: {1}°",
            Mathf.Round(strike).ToString("000"),
            Mathf.Round(dip).ToString("00")
            );

        }

        SetText(strBuilder.ToString());
    }

    // Called when a new stereonet is selected
    // Called for undo action
    // Called for delete all TODO
    public void Clear()
    {
    }
}
