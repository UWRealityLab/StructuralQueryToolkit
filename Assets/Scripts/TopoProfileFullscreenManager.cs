using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Application = UnityEngine.Application;

public class TopoProfileFullscreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI graphDataText;
    [SerializeField] private UILineRenderer graphRenderer;

    [SerializeField] private DownloadPopupIndicator downloadPopupIndicator;

    [Header("Text")] 
    [SerializeField] private TMP_Text yTopText;
    [SerializeField] private TMP_Text yBottomText;
    [SerializeField] private TMP_Text xText;
    
    private TopographicProfileMeasurement _activeProfile;
    private Vector2[] _graphPoints;
    private float _maxXPos;
    private float _maxYPos;

    private int _sigFigs;
    private NumberFormatInfo _nfi;
    

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            downloadPopupIndicator.SetText("Downloaded", "Also copied to clipboard");
        }
        else
        {
            downloadPopupIndicator.SetText("Copied to Clipboard", "");
        }
    }

    public void Setup(string titleName, TopographicProfileMeasurement activeProfile)
    {
        var sigFigs = TopographicProfileTool.instance.sigDigits;
        _nfi = new NumberFormatInfo();
        _nfi.NumberDecimalDigits = sigFigs;
        
        gameObject.SetActive(true);

        _activeProfile = activeProfile;
        activeProfile.GetGraphPoints(graphRenderer.rectTransform.rect.size, out _graphPoints, out _maxYPos, out _maxXPos);

        if (_graphPoints.Length == 0)
        {
            ClearScreen();
            return;
        }
        
        yBottomText.text = $"{_activeProfile.MinHeight.ToString("F", _nfi)}m";

        yTopText.text = $"{_activeProfile.MaxHeight.ToString("F", _nfi)}m";
        yTopText.rectTransform.anchoredPosition = new Vector2(yTopText.rectTransform.anchoredPosition.x, Mathf.Lerp(-graphRenderer.rectTransform.rect.height, 0f, _maxYPos));

        xText.text = $"{_activeProfile.TotalDistance.ToString("F", _nfi)}m";
        xText.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-graphRenderer.rectTransform.rect.width, 0f, _maxXPos), xText.rectTransform.anchoredPosition.y);

        var rawGraphPoints = activeProfile.GetRawGraphPoints();
        
        titleText.text = string.IsNullOrEmpty(titleName) ? "Profile" : titleName;
        graphDataText.text = MakeGraphTextList(rawGraphPoints);
        graphRenderer.Points = _graphPoints;
    }

    private string MakeGraphTextList(Vector2[] rawGraphPoints)
    {
        var sigFigs = TopographicProfileTool.instance.sigDigits;

        var nfi = new NumberFormatInfo();
        nfi.NumberDecimalDigits = sigFigs;
        
        var strBuilder = new StringBuilder();

        strBuilder.AppendLine($"Title: {titleText.text} \nTotal Samples: {rawGraphPoints.Length}\nx\ty");

        foreach (var rawGraphPoint in rawGraphPoints)
        {
            strBuilder.AppendLine($"{rawGraphPoint.x.ToString("F", nfi)}\t{rawGraphPoint.y.ToString("F", _nfi)}");
        }

        return strBuilder.ToString();
    }
    
    public void ExportGraphToTxt()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(graphDataText.text,$"{titleText.text}.txt");
        }

        GUIUtility.systemCopyBuffer = graphDataText.text;
        
        downloadPopupIndicator.ShowPopup();
    }

    // Contains the starting graph lines (x and y) 
    private string xmlHeader = @"<svg height=""1000"" width=""1000"">
<!--Y Axis-->
<line x1=""100"" y1=""50"" x2=""100"" y2=""950"" style=""stroke:black;stroke-width:2"" />
<line x1=""90"" y1=""50"" x2=""110"" y2=""50"" style=""stroke:black;stroke-width:1"" />
<!--X Axis-->
<line x1=""100"" y1=""950"" x2=""950"" y2=""950"" style=""stroke:black;stroke-width:2"" />
<line x1=""950"" y1=""940"" x2=""950"" y2=""960"" style=""stroke:black;stroke-width:1"" />
";
    
    private string pointsXmlHeader = "<polyline points=\"";

    public void ExportGraphToSVG()
    {
        var outputStrBuilder = new StringBuilder(xmlHeader);
        outputStrBuilder.AppendLine();
        
        // Adding the points
        outputStrBuilder.Append(pointsXmlHeader);
        foreach (var point in _graphPoints)
        {
            outputStrBuilder.Append($"{point.x * 850 + 100},{1000 - (point.y * 900 + 50)} ");
        }
        outputStrBuilder.AppendLine("\" style=\"fill:none;stroke:red;stroke-width:2\"/>");
        
        // Adding the text to the graph
        var yTopText = _activeProfile.MaxHeight;
        var yBottomText = _activeProfile.MinHeight;
        var xText = _activeProfile.TotalDistance;
        var xPos = Mathf.Lerp(0f, 975f, _maxXPos);
        var yPos = Mathf.Lerp(950f, 0f, _maxYPos);
        outputStrBuilder.AppendLine($"<text x=\"85\" y=\"{yPos}\" font-size=\"26\" text-anchor=\"end\" font-family=\"sans-serif\" fill=\"black\">{yTopText.ToString("F", _nfi)}m</text>" +
                                    $"\n<text x=\"85\" y=\"950\" font-size=\"26\" text-anchor=\"end\" font-family=\"sans-serif\" fill=\"black\">{yBottomText.ToString("F", _nfi)}m</text>\n" +
                                    $"<text x=\"{xPos}\" y=\"985\" font-size=\"26\" text-anchor=\"end\" font-family=\"sans-serif\" fill=\"black\">{xText.ToString("F", _nfi)}m</text>");
        
        
        // Ending
        outputStrBuilder.Append("</svg>");
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(outputStrBuilder.ToString(),$"{titleText.text}.xml");
        }
        GUIUtility.systemCopyBuffer = outputStrBuilder.ToString();
        downloadPopupIndicator.ShowPopup();
    }

    public void ClearScreen()
    {
        yTopText.text = "";
        yBottomText.text = "";
        xText.text = "";
        
        titleText.text = "Profile";
        graphDataText.text = "";
        graphRenderer.Points = new Vector2[] { Vector2.zero };

    }
}

