using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public static class StereonetExportUtils 
{

    public static LinkedList<MeasurementsGroup> GetMeasurementGroups(Stereonet stereonet)
    {
        StereonetFullscreenManager.Instance.UpdateValues("", stereonet as Stereonet2D, false);
        var measurementGroups = StereonetFullscreenManager.Instance.MeasurementGroups;
        return measurementGroups;
    }
    
    private static string GetListFormatText(Stereonet stereonet)
    {
        var outputStringBuilder = new StringBuilder();
        var measurementGroups = GetMeasurementGroups(stereonet);
        
        var stereonetName = stereonet.name.Equals("") ? $"{stereonet.id}: {stereonet.name}" : $"Stereonet #{stereonet.id}";
        outputStringBuilder.AppendLine(stereonetName);
        outputStringBuilder.AppendLine($"{stereonet.descriptionText}");

        // PLANES
        outputStringBuilder.Append("---Planes---");
        int numUnnamedPlaneGroups = 1;
        foreach (var group in measurementGroups)
        {
            if (group.GetType() == typeof(PlaneMeasurementGroup))
            {
                outputStringBuilder.AppendLine();
                
                var planeGroup = ((PlaneMeasurementGroup) group);
                var measurements = planeGroup.StrikeAndDipArr;
                var (avgStrike, avgDip) = planeGroup.AverageStrikeAndDip();
                if (measurements == null || measurements.Count == 0)
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
        var stereonetData = stereonet.GetAvgStereonetPoleData();
        var elevationData = stereonet.poleElevations;
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
        foreach (var group in measurementGroups)
        {
            if (group.GetType() == typeof(LineMeasurementGroup))
            {
                outputStringBuilder.AppendLine();
                
                var lineGroup = ((LineMeasurementGroup) group);
                var measurements = lineGroup.TrendAndPlungeArr;
                var (avgTrend, avgPlunge) = lineGroup.AverageTrendAndPlunge();
                if (measurements == null || measurements.Count == 0)
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
    
    
    private const string STEREONET_MOBILE_FORMAT_HEADER = "No.	Type	Structure	Color	Trd/Strk	Plg/Dip	Longitude	Latitude	Horiz ± m	Elevation	Elev ± m	Time	Day	Month	Year	Notes	Checked	Strabo Type	Strabo Quality	Strabo Plane Detail	Strabo Plane Addl Detail	Plane Thickness	Plane Length	Geologist";

    private static string GetStereonetMobileFormatText(Stereonet stereonet)
    {
        // TODO redundant calculations
        var stereonetMobileFormatStringBuilder = new StringBuilder();
        stereonetMobileFormatStringBuilder.AppendLine(STEREONET_MOBILE_FORMAT_HEADER);
        
        var measurementGroups = GetMeasurementGroups(stereonet);
        
        // Poles (only has one group)
        var stereonetData = stereonet.GetAvgStereonetPoleData();
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
        foreach (var group in measurementGroups)
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
    

    public static void DownloadToListFormat(Stereonet stereonet)
    {
        var str = GetListFormatText(stereonet);
        
#if !UNITY_ANDROID
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(str,$"{stereonet.name}.txt");
        }
#else
        File.WriteAllBytes($"{Application.persistentDataPath}/{stereonet.id}_{stereonet.name}.txt", Encoding.UTF8.GetBytes(str));
        
        //File.WriteAllBytes($"/storage/emulated/0/Download/testWrite.png", mapTex.EncodeToPNG());
#endif

    }

    public static void DownloadToListFormat(List<Stereonet> stereonets)
    {
        var strBuilder = new StringBuilder();

        foreach (var stereonet in stereonets)
        {
            strBuilder.AppendLine(GetListFormatText(stereonet));
        }
        
#if !UNITY_ANDROID
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //WebGLFileSaver.SaveFile(strBuilder.ToString(),$"{titleText.text}.txt");
        }
#else
        Debug.Log($"{Application.persistentDataPath}");
        File.WriteAllBytes($"{Application.persistentDataPath}/Stereonets.txt", Encoding.UTF8.GetBytes(strBuilder.ToString()));
        
        //File.WriteAllBytes($"/storage/emulated/0/Download/testWrite.png", mapTex.EncodeToPNG());
#endif
    }
    
    /// <summary>
    /// Registers point or line measurement to be later exported into Stereonet Mobile Format
    /// </summary>
    private static void RegisterPointOrLineMeasurement(StringBuilder strBuilder, int index, string groupName, float trend, float plunge)
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
    private static void RegisterPlaneMeasurement(StringBuilder strBuilder, int index, string groupName, float strike, float dip)
    {
        if (groupName.Equals(""))
        {
            groupName = "Default Planes Dataset";
        }

        strBuilder.AppendLine($"{index}\tP\t{groupName}	{000000000}	{strike}	{dip}	999	99		0		1:00:	1	1	2021	_	1	_	0	_	_	_	_	_");
    }
}
