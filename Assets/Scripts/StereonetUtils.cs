using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class StereonetUtils
{
    public static void CalculateStrikeAndDip(Vector3 vector, out float strike, out float dip)
    {
        float trend;
        float plunge;
        
        if (vector.y <= 0f)
        {
            vector = -vector;
        }

        trend = (Mathf.Atan2(vector.x, vector.z) * 180 / Mathf.PI);
            
        if (vector.z < 0f)
        {
             trend += 180;
        }
        if (trend < 0f)
        {
            trend += 180;
        }

        plunge = (Mathf.Asin(vector.y) * 180 / Mathf.PI);
        
        strike = trend + 90;
        dip = 90 - plunge;

        // Minus strike by 360 if it's larger than 360
        if (strike >= 360f)
        {
            strike -= 360;
        }

        if (vector.z > 0f && strike < 180f)
        {
            strike += 180;
        }
    }

    public static void CalculateTrendAndPlunge(Vector3 vector, out float trend, out float plunge)
    {
        if (vector.y < 0f)
        {
            vector = -vector;
        }

        trend = (Mathf.Atan2(vector.x, vector.z) * 180 / Mathf.PI);

        trend += 180;

        // Plunge
        plunge = (Mathf.Asin(vector.y) * 180 / Mathf.PI);
    }

    public static void CalculateTrendAndPlunge(float strike, float dip, out float trend, out float plunge)
    {
        trend = strike - 90f;
        plunge = 90f - dip;
    }
}
