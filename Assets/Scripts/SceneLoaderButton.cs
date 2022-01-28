using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderButton : MonoBehaviour
{
    private string sceneName;
    
    #if UNITY_EDITOR
    public SceneAsset scene;

    private void OnValidate()
    {
        if (scene)
        {
            sceneName = scene.name;
        }
    }
    #endif

    public void PlayScene()
    {
        MainMenu.instance.PlayScene(sceneName);
    }
}
