using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This attribute contains simple data to be used by the PropertyDrawer
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class DrawIfAttribute : PropertyAttribute
{
    #region Fields

    public string comparedPropertyName { get; private set; }
    public object comparedValue { get; private set; }
    public DisablingType disablingType { get; private set; }


    /// <summary>
    /// Types of disabling you'll see in the inspector.
    /// ReadOny dims the fields. 
    /// DonTDraw hides the fields completely.
    /// </summary>
    public enum DisablingType {
        ReadOnly = 2,
        DontDraw = 3
    }

    #endregion

    /// <summary>
    /// Only draws the field only if a condition is met. Supports enum and bools.
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    /// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
    public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw) {
        this.comparedPropertyName = comparedPropertyName;
        this.comparedValue = comparedValue;
        this.disablingType = disablingType;
    }
}
