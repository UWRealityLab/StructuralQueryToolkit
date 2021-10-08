using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A group shown in the fullscreen UI
/// </summary>
public class MeasurementsGroup : MonoBehaviour
{
    public TMP_InputField groupNameText;

    public TextMeshProUGUI avgDataText;

    // List containing all the measurements' strike and dip
    public TextMeshProUGUI measurementsListText;

    [HideInInspector]
    public Measurement measurement;


    /// <summary>
    /// Should be executed when the group name input field changes
    /// </summary>
    /// <param name="name"></param>
    public void SetMeasurementName(TMP_InputField inputText)
    {
        measurement.measurementName = inputText.text;
    }

    public void SetGroupTitleText(string groupName)
    {
        groupNameText.text = groupName;
    }
}