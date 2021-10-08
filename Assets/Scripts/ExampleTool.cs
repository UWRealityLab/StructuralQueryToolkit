using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTool : PlayerTool
{
    
    public override void UseTool(RaycastHit hit)
    {
        var latestMeasurementText = $"Normal: {hit.point.x.ToString("0.0")}, {hit.point.y.ToString("0.0")}, {hit.point.z.ToString("0.0")}";
        LatestMeasurementUI.instance.SetText(latestMeasurementText);
    }

    public override void Undo()
    {
        
    }
}
