using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour {

    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI progressText;

    // Loads our current whale back project
    public void LoadLevel (int sceneIndex) {
        StartCoroutine(Load(sceneIndex));
    
    }

    // Creates a fancy loading screen while whaleback is loaded async.
    IEnumerator Load (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            slider.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

            yield return null;
        }
    }
}
