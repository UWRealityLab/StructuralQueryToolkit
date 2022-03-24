using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneLoaderButton : MonoBehaviour
{
    [HideInInspector] public string sceneName;
    
    public bool UseSceneIndex = false;
    [DrawIf(nameof(UseSceneIndex), true, DrawIfAttribute.DisablingType.DontDraw), SerializeField]
    public int SceneIndex = -1;
    
#if UNITY_EDITOR
    public SceneAsset scene;
    
    private void OnValidate()
    {
        if (scene)
        {
            sceneName = scene.name;
        }
        else
        {
            Debug.LogError("No reference to scene asset");
        }
    }
#endif

    public void PlayScene()
    {
        if (sceneName == null || sceneName.Equals("null") || sceneName.Equals(""))
        {
            Debug.LogError("No reference to scene");
        }
        
        if (UseSceneIndex)
        {
            MainMenu.instance.PlayScene(SceneIndex);
        }
        else
        {
            MainMenu.instance.PlayScene(sceneName);
        }
    }
}