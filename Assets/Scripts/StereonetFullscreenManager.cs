using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class StereonetFullscreenManager : MonoBehaviour
{
    private static StereonetFullscreenManager _instance;

    public static StereonetFullscreenManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StereonetFullscreenManager>(true);
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public UnityEvent<MeasurementsGroup> OnAddMeasurementGroup;
    
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Transform stereonetImage2D;
    [SerializeField] GameObject steroenetImage3D;

    [Header("Pole measurements data")]
    [SerializeField] TextMeshProUGUI avgPoleDataText;
    [SerializeField] TextMeshProUGUI poleMeasurementsText;

    [Header("Plane/Lineation Group Parents")]
    [SerializeField] Transform planeGroupParent;
    [SerializeField] Transform lineationGroupParent;

    [Header("Measurement groups prefabs")]
    [SerializeField] GameObject planeGroupPrefab;
    [SerializeField] GameObject lineationGroupPrefab;

    [Header("For refreshing layout")]
    [SerializeField] LayoutRebuild layoutRebuild;

    // Storing references to created groups to clear later
    public LinkedList<MeasurementsGroup> MeasurementGroups;
    
    private Stereonet _activeStereonet;
    [HideInInspector] public UnityEvent OnCloseEvent;
    
    [Header("Email Exporting")]
    [SerializeField] private TMP_InputField _emailToInputText;
    [SerializeField] private TMP_InputField _stereonetDescriptionInputText;
    [SerializeField] private RenderTexture _mapViewRT;
    
    
    void Awake()
    {
        Instance = this;
        MeasurementGroups = new LinkedList<MeasurementsGroup>();
    }

    public void UpdateValues(string title, Stereonet2D stereonet, bool updateLayout = true)
    {
        Reset();
        
        if (updateLayout)
        {
            stereonet.MoveStereonetUI(stereonetImage2D);

            titleText.text = string.IsNullOrWhiteSpace(title) ? "Stereonet" : title;

            if (_stereonetDescriptionInputText != null)
            {
                _stereonetDescriptionInputText.text = stereonet.descriptionText;
            }
        }
        _activeStereonet = stereonet;

        SetPoleData();
        SetPlaneData();
        SetLineationData();

        if (updateLayout)
        {
            layoutRebuild.RebuildLayout();
        }
    }
    
    /// <summary>
    /// Calculate the average strike and dip
    /// Creates info cards for each group of measurements
    /// </summary>
    public void SetPlaneData()
    {
        // By default, there will always be one group for the non-combined data points
        var defaultGroup = CreateMeasurementGroupUI<PlaneMeasurementGroup>(planeGroupPrefab, planeGroupParent, _activeStereonet.defaultPlaneMeasurement);
        
        StringBuilder defaultGroupStrBuilder = new StringBuilder();

        Vector3 defaultGroupAvgNormal = Vector3.zero;
        int numNonCombinedPlanes = 0;

        var planeNode = _activeStereonet.stereonetPlanes.First;
        while (planeNode != null)
        {
            var piPlotPlane =  planeNode.Value;
            if (piPlotPlane.isCombined)
            {
                // If the stereonet plane is a combined line, then create a new group
                var newPlaneGroup = CreateMeasurementGroupUI<PlaneMeasurementGroup>(planeGroupPrefab, planeGroupParent, piPlotPlane);
                
                float currGroupAvgStrike = 0f;
                float currGroupAvgDip = 0f;
                StringBuilder strBuilder = new StringBuilder(); // String the list of measurements
                foreach (var childPlane in piPlotPlane.combinedPlaneLines)
                {
                    currGroupAvgStrike += childPlane.strike;
                    currGroupAvgDip += childPlane.dip;
                    strBuilder.AppendFormat("{0},{1}\t", childPlane.strike.ToString("000"), childPlane.dip.ToString("00"));
                }
                newPlaneGroup.measurementsListText.text = strBuilder.ToString();
                
                // For exporting
                var strikeDipArr = piPlotPlane.combinedPlaneLines.ToArray();
                for (int i = 0; i < strikeDipArr.Length; i++)
                {
                    newPlaneGroup.StrikeAndDipArr.Add((strikeDipArr[i].strike, strikeDipArr[i].dip));
                }
                
                currGroupAvgStrike /= piPlotPlane.combinedPlaneLines.Count;
                currGroupAvgDip /= piPlotPlane.combinedPlaneLines.Count;
                newPlaneGroup.avgDataText.text = string.Format("Average strike and dip: {0}, {1}", currGroupAvgStrike.ToString("000"), currGroupAvgDip.ToString("00"));
            }
            else
            {
                // Else get its strike/dip and append it to the 1st group
                // and sum up the poles to later calculate the average strike/dip
                defaultGroupAvgNormal += piPlotPlane.GetForwardDirection();

                // For exporting
                defaultGroup.StrikeAndDipArr.Add((piPlotPlane.strike, piPlotPlane.dip));
                //RegisterPlaneMeasurement(numNonCombinedPlanes, defaultGroup.groupNameText.text, piPlotPlane.strike, piPlotPlane.dip);

                numNonCombinedPlanes++;
                defaultGroupStrBuilder.AppendFormat("{0},<space=5>{1}\t", piPlotPlane.strike.ToString("000"), piPlotPlane.dip.ToString("00"));
            }

            planeNode = planeNode.Next;
        }

        // List measurements for default group
        defaultGroup.measurementsListText.text = defaultGroupStrBuilder.ToString();

        // Avg measurements for the default group
        if (numNonCombinedPlanes > 0)
        {
            defaultGroupAvgNormal /= numNonCombinedPlanes;
            float avgPlaneStrike;
            float avgPlaneDip;
            StereonetUtils.CalculateStrikeAndDip(defaultGroupAvgNormal, out avgPlaneStrike, out avgPlaneDip);
            defaultGroup.avgDataText.text = string.Format("Average strike and dip: {0}, {1}", avgPlaneStrike.ToString("000"), avgPlaneDip.ToString("00"));
        }
        else
        {
            defaultGroup.avgDataText.text = "Average strike and dip: 0, 0";
        }
    }
    
    public void SetPoleData()
    {
        // Writing to Pole Field Measurements
        StringBuilder strBuilder = new StringBuilder();
        var stereonetData = _activeStereonet.GetAvgStereonetPoleData();
        foreach (var pair in stereonetData.strikeDipPairs)
        {
            strBuilder.AppendFormat("{0},{1}\t", Mathf.Round(pair.Item1).ToString("000"), Mathf.Round(pair.Item2).ToString("00"));
        }
        poleMeasurementsText.text = strBuilder.ToString();
        
        // Writing to Pi-Plot Pole Fold Axis
        strBuilder.Clear();
        if (stereonetData.strikeDipPairs.Count > 0)
        {
            Assert.IsTrue(stereonetData.avgPoleTrend > 0f && stereonetData.avgPolePlunge >= 0f);
            strBuilder.AppendFormat("Pi-Plot Trend, Plunge: {0}, {1}", Mathf.Round(stereonetData.avgPoleTrend), Mathf.Round(stereonetData.avgPolePlunge));
        }
        else
        {
            strBuilder.AppendFormat("Pi-Plot Trend, Plunge: 0, 0");
        }
        avgPoleDataText.text = strBuilder.ToString();
    }

    public void SetLineationData()
    {
        // By default, there will always be one group for the non-combined
        // data points
        var defaultGroup = CreateMeasurementGroupUI<LineMeasurementGroup>(lineationGroupPrefab, lineationGroupParent, _activeStereonet.defaultLineationMeasurement);

        StringBuilder defaultGroupStrBuilder = new StringBuilder();

        Vector3 defaultGroupAvgNormal = Vector3.zero;
        int numNonCombinedLines = 0;

        var lineNode = _activeStereonet.stereonetLinearPoints.First;
        int defaultGroupSize = 0;

        while (lineNode != null)
        {
            var piPlotLine = lineNode.Value;
            if (piPlotLine.isCombined)
            {
                // If the stereonet plane is a combined line, then create a new group
                var newLineationGroup = CreateMeasurementGroupUI<LineMeasurementGroup>(lineationGroupPrefab, lineationGroupParent, piPlotLine);

                float currGroupTrend = 0f;
                float currGroupPlunge = 0f;
                StringBuilder strBuilder = new StringBuilder(); // String the list of measurements
                foreach (var childPlane in piPlotLine.combinedStereonetPoints)
                {
                    currGroupTrend += childPlane.trend;
                    currGroupPlunge += childPlane.plunge;
                    strBuilder.AppendFormat("{0},<space=5>{1}\t", childPlane.trend.ToString("000"), childPlane.plunge.ToString("00"));
                }
                newLineationGroup.measurementsListText.text = strBuilder.ToString();

                // For exporting
                var trendAndPlungeArr = piPlotLine.combinedStereonetPoints.ToArray();
                for (int i = 0; i < trendAndPlungeArr.Length; i++)
                {
                    newLineationGroup.TrendAndPlungeArr.Add((trendAndPlungeArr[i].trend, trendAndPlungeArr[i].plunge));
                }
                
                currGroupTrend /= piPlotLine.combinedStereonetPoints.Count;
                currGroupPlunge /= piPlotLine.combinedStereonetPoints.Count;
                newLineationGroup.avgDataText.text = string.Format("Average Trend and Plunge: {0}, {1}", currGroupTrend.ToString("000"), currGroupPlunge.ToString("00"));
            }
            else
            {
                // Else get its trend/plunge and append it to the 1st group
                // and sum up the poles to later calculate the average trend/plunge
                defaultGroupAvgNormal += piPlotLine.transform.forward;
                numNonCombinedLines++;
                
                // For exporting
                defaultGroup.TrendAndPlungeArr.Add((piPlotLine.trend, piPlotLine.plunge));

                defaultGroupStrBuilder.AppendFormat("{0},<space=5>{1}\t", piPlotLine.trend.ToString("000"), piPlotLine.plunge.ToString("00"));
            }
            lineNode = lineNode.Next;
        }


        // List measurements for default group
        defaultGroup.measurementsListText.text = defaultGroupStrBuilder.ToString();

        // Avg measurements for the default group
        if (numNonCombinedLines > 0)
        {
            defaultGroupAvgNormal /= numNonCombinedLines;
            float avgTrend;
            float avgPlunge;
            StereonetUtils.CalculateTrendAndPlunge(defaultGroupAvgNormal, out avgTrend, out avgPlunge);
            defaultGroup.avgDataText.text = string.Format("Average Trend and Plunge: {0}, {1}", avgTrend.ToString("000"), avgPlunge.ToString("00"));
        }
        else
        {
            defaultGroup.avgDataText.text = "Average Trend and Plunge: 0, 0";
        }
    }
    
    private T CreateMeasurementGroupUI<T>(GameObject groupPrefab, Transform groupParent, Measurement measurement) where T : MeasurementsGroup
    {
        var newMeasurementGroup = Instantiate(groupPrefab, groupParent).GetComponent<T>();
        MeasurementGroups.AddLast(newMeasurementGroup);
        newMeasurementGroup.measurement = measurement;
        newMeasurementGroup.SetGroupTitleText(measurement.measurementName);
        
        OnAddMeasurementGroup.Invoke(newMeasurementGroup);

        return newMeasurementGroup;
    }

    public void Close()
    {
        OnCloseEvent.Invoke();
        Reset();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears everything 
    /// </summary>
    public void Reset()
    {
        foreach (var group in MeasurementGroups)
        {
            Destroy(group.gameObject);
        }
        MeasurementGroups.Clear();
    }
    
    private const string STEREONET_MOBILE_FORMAT_HEADER = "No.	Type	Structure	Color	Trd/Strk	Plg/Dip	Longitude	Latitude	Horiz ± m	Elevation	Elev ± m	Time	Day	Month	Year	Notes	Checked	Strabo Type	Strabo Quality	Strabo Plane Detail	Strabo Plane Addl Detail	Plane Thickness	Plane Length	Geologist";
    public void ExportToStereonetMobileFormat()
    {

        var output = GetStereonetMobileFormatText();
        Debug.Log(output);

#if !UNITY_ANDROID
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(output,$"{titleText.text}.txt");
        }
#endif
        
        GUIUtility.systemCopyBuffer = output;

    }

    private string GetStereonetMobileFormatText()
    {
        // TODO redundant calculations
        var stereonetMobileFormatStringBuilder = new StringBuilder();
        stereonetMobileFormatStringBuilder.AppendLine(STEREONET_MOBILE_FORMAT_HEADER);
        
        // Poles (only has one group)
        var stereonetData = _activeStereonet.GetAvgStereonetPoleData();
        var curr = stereonetData.trendPlungePairs.First;
        for (int i = 0; i < stereonetData.trendPlungePairs.Count; i++)
        {
            var pair = curr.Value;
            RegisterPointOrLineMeasurement(stereonetMobileFormatStringBuilder, i, "Poles Dataset", pair.Item1, pair.Item2);
            curr = curr.Next;
        }

        
        // Lines and Planes
        int numUnnamedPlaneGroups = 0;
        int numUnnamedLineGroups = 0;
        foreach (var group in MeasurementGroups)
        {
            if (group.GetType() == typeof(LineMeasurementGroup))
            {
                var measurements = ((LineMeasurementGroup) group).TrendAndPlungeArr;
                if (measurements.Count == 0)
                {
                    continue;
                }
                string groupName = group.groupNameText.text;
                if (groupName.Equals(""))
                {
                    groupName = $"Unnamed Lines Dataset {numUnnamedLineGroups++}";
                }
                for (int i = 0; i < measurements.Count; i++)
                {
                    var measurement = measurements[i];
                    RegisterPointOrLineMeasurement(stereonetMobileFormatStringBuilder, i, groupName, measurement.Item1, measurement.Item2);
                }
            }
            else if (group.GetType() == typeof(PlaneMeasurementGroup))
            {
                var measurements = ((PlaneMeasurementGroup) group).StrikeAndDipArr;
                if (measurements.Count == 0)
                {
                    continue;
                }

                string groupName = group.groupNameText.text;
                if (groupName.Equals(""))
                {
                    groupName = $"Unnamed Planes Dataset {numUnnamedPlaneGroups++}";
                }

                for (int i = 0; i < measurements.Count; i++)
                {
                    var measurement = measurements[i];
                    RegisterPlaneMeasurement(stereonetMobileFormatStringBuilder, i, groupName, measurement.Item1, measurement.Item2);
                }
            }
            else
            {
                Debug.LogError("Invalid measurement group type");
            }
        }

        return stereonetMobileFormatStringBuilder.ToString();
    }
    
    /// <summary>
    /// Exports to a custom designed text file for the selected stereonet
    /// </summary>
    public void ExportToListFormat()
    {
        var str = GetListFormatText(_activeStereonet);
        Debug.Log(str);

        var mapTex = GetMapTex();

#if !UNITY_ANDROID
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(str,$"{titleText.text}.txt");
        }
#else
        print($"{Application.persistentDataPath}");
        File.WriteAllBytes($"{Application.persistentDataPath}/{titleText.text}.png", mapTex.EncodeToPNG());
        File.WriteAllBytes($"{Application.persistentDataPath}/{titleText.text}.txt", Encoding.UTF8.GetBytes(str));
        
        //File.WriteAllBytes($"/storage/emulated/0/Download/testWrite.png", mapTex.EncodeToPNG());
#endif

        GUIUtility.systemCopyBuffer = str;

        //StartCoroutine(ExportToListFormatCo());
    }

    public void DownloadToListFormat(Stereonet stereonet)
    {
        var str = GetListFormatText(stereonet);


#if !UNITY_ANDROID
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(str,$"{titleText.text}.txt");
        }
#else
        print($"{Application.persistentDataPath}");
        File.WriteAllBytes($"{Application.persistentDataPath}/{stereonet.id}_{titleText.text}.txt", Encoding.UTF8.GetBytes(str));
        
        //File.WriteAllBytes($"/storage/emulated/0/Download/testWrite.png", mapTex.EncodeToPNG());
#endif

    }

    public void DownloadMap()
    {
#if UNITY_ANDROID
        var mapTex = GetMapTex();
        File.WriteAllBytes($"{Application.persistentDataPath}/map.png", mapTex.EncodeToPNG());
#endif
    }
    
    private IEnumerator DownloadToListFormat()
    {
        yield return new WaitForEndOfFrame();
        var str = GetListFormatText(_activeStereonet);
        Debug.Log(str);

        var mapTex = GetMapTex();

#if !UNITY_ANDROID
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(str,$"{titleText.text}.txt");
        }
#else
        print($"{Application.persistentDataPath}");
        File.WriteAllBytes($"{Application.persistentDataPath}/{titleText.text}.png", mapTex.EncodeToPNG());
        File.WriteAllBytes($"{Application.persistentDataPath}/{titleText.text}.txt", Encoding.UTF8.GetBytes(str));
        
        //File.WriteAllBytes($"/storage/emulated/0/Download/testWrite.png", mapTex.EncodeToPNG());
#endif

        GUIUtility.systemCopyBuffer = str;
    }

    private Texture2D GetMapTex()
    {
        RenderTexture.active = _mapViewRT;
        var mapTex = new Texture2D(_mapViewRT.width, _mapViewRT.height, TextureFormat.RGBAFloat, false);
        mapTex.ReadPixels(new Rect(0, 0, _mapViewRT.width, _mapViewRT.height), 0, 0);
        
        //Graphics.CopyTexture(_mapViewRT, mapTex);
        
        mapTex.Apply();

        return mapTex;
    }

    private string GetListFormatText(Stereonet stereonet)
    {
        var outputStringBuilder = new StringBuilder();
        
        var stereonetName = stereonet.name.Equals("") ? $"{stereonet.id}: {stereonet.name}" : $"{stereonet.id}: Stereonet";
        outputStringBuilder.AppendLine($"Stereonet: {stereonetName}\n");
        
        // PLANES
        outputStringBuilder.Append("---Planes---");
        int numUnnamedPlaneGroups = 1;
        foreach (var group in MeasurementGroups)
        {
            if (group.GetType() == typeof(PlaneMeasurementGroup))
            {
                outputStringBuilder.AppendLine();
                
                var planeGroup = ((PlaneMeasurementGroup) group);
                var measurements = planeGroup.StrikeAndDipArr;
                var (avgStrike, avgDip) = planeGroup.AverageStrikeAndDip();
                if (measurements.Count == 0)
                {
                    continue;
                }

                var groupName = group.groupNameText.text.Equals("") ? $"{numUnnamedPlaneGroups++}" : group.groupNameText.text;
                
                outputStringBuilder.AppendLine($"Group: {groupName}");
                outputStringBuilder.AppendLine($"Average strike and dip:\t{avgStrike.ToString("000")}, {avgDip.ToString("00")}");
                outputStringBuilder.AppendLine("Measurements:\nStrike\tDip");
                for (int i = 0; i < measurements.Count; i++)
                {
                    var measurement = measurements[i];
                    outputStringBuilder.AppendLine($"{measurement.Item1.ToString("000")}\t{measurement.Item2.ToString("00")}");
                }
            }
        }
        outputStringBuilder.AppendLine();

        // POLES (only has one group)
        var stereonetData = _activeStereonet.GetAvgStereonetPoleData();
        var elevationData = _activeStereonet.poleElevations;
        outputStringBuilder.AppendLine($"---Poles to Planes w/ elevation---");
        if (stereonetData.strikeDipPairs.Count > 0)
        {
            outputStringBuilder.AppendLine($"Pi-Plot Trend, Plunge:\t{Mathf.Round(stereonetData.avgPoleTrend)}\t{Mathf.Round(stereonetData.avgPolePlunge)}");
            outputStringBuilder.AppendLine("Strike\tDip\tElevation");
            var curr = stereonetData.strikeDipPairs.First;
            for (int i = 0; i < stereonetData.strikeDipPairs.Count; i++)
            {
                var pair = curr.Value;
                var currElevation = Settings.instance.showElevationData ? System.Math.Round(elevationData[i] + Settings.instance.elevationBias, 2).ToString() : "n/a";
                outputStringBuilder.AppendLine($"{pair.Item1.ToString("000")}\t{pair.Item2.ToString("00")}\t{currElevation}");

                curr = curr.Next;
            }
        }
        outputStringBuilder.AppendLine();

        // LINEATIONS
        outputStringBuilder.Append("---Lineations---");
        int numUnnamedLineGroups = 1;
        foreach (var group in MeasurementGroups)
        {
            if (group.GetType() == typeof(LineMeasurementGroup))
            {
                outputStringBuilder.AppendLine();
                
                var lineGroup = ((LineMeasurementGroup) group);
                var measurements = lineGroup.TrendAndPlungeArr;
                var (avgTrend, avgPlunge) = lineGroup.AverageTrendAndPlunge();
                if (measurements.Count == 0)
                {
                    continue;
                }

                var groupName = group.groupNameText.text.Equals("") ? $"{numUnnamedLineGroups++}" : group.groupNameText.text;
                
                outputStringBuilder.AppendLine($"Group: {groupName}");
                outputStringBuilder.AppendLine($"Average trend and plunge:\t{Mathf.Round(avgTrend)}\t{Mathf.Round(avgPlunge)}");
                outputStringBuilder.AppendLine("Measurements:\nTrend\tPlunge");
                for (int i = 0; i < measurements.Count; i++)
                {
                    var measurement = measurements[i];
                    outputStringBuilder.AppendLine($"{measurement.Item1.ToString("000")}\t{measurement.Item2.ToString("00")}");
                }
            }
        }

        return outputStringBuilder.ToString();
    }
    
    public void InitializeMap()
    {
        MapView.instance.mapViewCamera.Render();
    }

    public void SendEmail()
    {
        var mapTex = GetMapTex();
        
        var fromEmailAddress = new MailAddress("virtualfieldgeology@gmail.com");
        var toEmailAddress = new MailAddress(string.IsNullOrEmpty(_emailToInputText.text) ? "virtualfieldgeology@gmail.com" : _emailToInputText.text);
        var mailMessage = new MailMessage(fromEmailAddress, toEmailAddress);
        mailMessage.Subject = titleText.text;
        mailMessage.Body = GetListFormatText(_activeStereonet);

        var mapBytes = mapTex.EncodeToPNG();
        var memoryStream = new MemoryStream(mapBytes);
        var mapAttachment = new Attachment(memoryStream, "Map.png");
        mailMessage.Attachments.Add(mapAttachment);

        var smtpClient = new SmtpClient("smtp.gmail.com");
        smtpClient.Timeout = 10000;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Port = 587;

        smtpClient.Credentials = new NetworkCredential("virtualfieldgeology@gmail.com", "whaleback123");
        smtpClient.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        };
        smtpClient.SendCompleted += (sender, args) =>
        {
            
        };

        mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        smtpClient.SendMailAsync(mailMessage);
        
        
        //StartCoroutine(SendEmailCo());
    }

    private IEnumerator SendEmailCo()
    {
        yield return new WaitForEndOfFrame();

        var mapTex = GetMapTex();
        
        var fromEmailAddress = new MailAddress("virtualfieldgeology@gmail.com");
        var toEmailAddress = new MailAddress(string.IsNullOrEmpty(_emailToInputText.text) ? "jackymooc@gmail.com" : _emailToInputText.text);
        var mailMessage = new MailMessage(fromEmailAddress, toEmailAddress);
        mailMessage.Subject = titleText.text;
        mailMessage.Body = GetListFormatText(_activeStereonet);

        var mapBytes = mapTex.EncodeToPNG();
        var memoryStream = new MemoryStream(mapBytes);
        var mapAttachment = new Attachment(memoryStream, "Map.png");
        mailMessage.Attachments.Add(mapAttachment);

        var smtpClient = new SmtpClient("smtp.gmail.com");
        smtpClient.Timeout = 10000;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Port = 587;

        smtpClient.Credentials = new NetworkCredential("virtualfieldgeology@gmail.com", "whaleback123");
        smtpClient.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        };
        smtpClient.SendCompleted += (sender, args) =>
        {
            
        };

        mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        smtpClient.SendMailAsync(mailMessage);


    }

    /// <summary>
    /// Registers point or line measurement to be later exported into Stereonet Mobile Format
    /// </summary>
    private void RegisterPointOrLineMeasurement(StringBuilder strBuilder, int index, string groupName, float trend, float plunge)
    {
        if (groupName.Equals(""))
        {
            groupName = "Default Lines Dataset";
        }
        
        strBuilder.AppendLine($"{index}\tL\t{groupName}	{000000000}	{trend}	{plunge}	999	99		0		1:00:	1	1	2021	_	1	_	0	_	_	_	_	_");
    }
    
    /// <summary>
    /// Registers plane measurement to be later exported into Stereonet Mobile Format
    /// </summary>
    private void RegisterPlaneMeasurement(StringBuilder strBuilder, int index, string groupName, float strike, float dip)
    {
        if (groupName.Equals(""))
        {
            groupName = "Default Planes Dataset";
        }

        strBuilder.AppendLine($"{index}\tP\t{groupName}	{000000000}	{strike}	{dip}	999	99		0		1:00:	1	1	2021	_	1	_	0	_	_	_	_	_");
    }

    public void ToggleStereonetView()
    {
        stereonetImage2D.gameObject.SetActive(!stereonetImage2D.gameObject.activeSelf);
        steroenetImage3D.SetActive(!steroenetImage3D.activeSelf);
    }
    
    public void UpdateStereonetDescription(TMP_InputField inputField)
    {
        _activeStereonet.SetDescriptionText(inputField.text);
    }
}
