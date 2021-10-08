using ICSharpCode.NRefactory.PrettyPrinter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
    // Reference to attribute of the property
    DrawIfAttribute drawIfAttribute;

    // Field that will be compared
    SerializedProperty comparedField;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // If true, then draw the property
        if (CheckCondition(property)) {
            EditorGUI.PropertyField(position, property);
        } 
        // If the disabling type is read-only, then draw it as disabled instead
        else if (drawIfAttribute.disablingType == DrawIfAttribute.DisablingType.ReadOnly) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="property"></param>
    /// <returns> True on error. True if the condition is met. False if the condition is not met </returns>
    public bool CheckCondition(SerializedProperty property) {

        drawIfAttribute = (DrawIfAttribute)attribute;

        string path = drawIfAttribute.comparedPropertyName;

        comparedField = property.serializedObject.FindProperty(path);

        if (comparedField == null) {
            Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        switch (comparedField.type) { // Possible extend cases to support your own type
            case "bool":
            return comparedField.boolValue.Equals(drawIfAttribute.comparedValue);
            case "Enum":
            return comparedField.enumValueIndex.Equals((int)drawIfAttribute.comparedValue);
            default:
            Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
            return true;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!CheckCondition(property) && drawIfAttribute.disablingType == DrawIfAttribute.DisablingType.DontDraw)
            return 0f;

        // The height of the property should be defaulted to the default height.
        return base.GetPropertyHeight(property, label);
    }
}
