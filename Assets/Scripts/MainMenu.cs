using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance; 
    
    [SerializeField] GameObject loadingIcon;

    private void Awake()
    {
        instance = this;
    }

    public void PlayScene(String sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        loadingIcon.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PlayScene(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
        loadingIcon.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }

}