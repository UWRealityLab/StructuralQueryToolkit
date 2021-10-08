using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LayoutRebuild : MonoBehaviour
{

    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        RebuildLayout();
    }

    private void OnEnable()
    {
        RebuildLayout();
    }

    public void MarkForRebuild()
    {
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    public void RebuildLayout() {
        StartCoroutine(RebuildLayoutCo());

    }

    private IEnumerator RebuildLayoutCo()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

        yield return new WaitForEndOfFrame();

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LayoutRebuild))]
public class AlignToNormalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LayoutRebuild myScript = (LayoutRebuild)target;
        if (GUILayout.Button("Rebuild Layout"))
        {
            myScript.RebuildLayout();
        }
    }
}
#endif