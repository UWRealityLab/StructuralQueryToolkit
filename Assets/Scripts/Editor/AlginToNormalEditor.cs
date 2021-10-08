using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlignToNormal))]
public class AlignToNormalEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        AlignToNormal myScript = (AlignToNormal)target;
        if (GUILayout.Button("Align with Backward Direction")) {
            myScript.AlignWithRaycast();
        }
    }
}