using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseMenu : MonoBehaviour
{
	private float m_TimeScaleRef = 1f;
    private float m_VolumeRef = 1f;
    private bool m_Paused;

    [SerializeField] GameObject pauseCanvas;
    [SerializeField] FirstPersonController playerController;

    private void Start()
    {
    }

    private void MenuOn ()
    {
        pauseCanvas.SetActive(true);
        GameController.instance.DisablePlayer();

        //m_TimeScaleRef = Time.timeScale;
        //Time.timeScale = 0f;

        m_VolumeRef = AudioListener.volume;
        AudioListener.volume = 0f;

        m_Paused = true;
        
        ToolManager.instance.DisableActiveTool();
    }


    public void MenuOff ()
    {
        GameController.instance.EnablePlayer();

        pauseCanvas.SetActive(false);
        playerController.enabled = true;

        //Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;
        m_Paused = false;
    }


    public void OnMenuStatusChange ()
    {
        if (!m_Paused)
        {
            MenuOn();
        }
        else if (m_Paused)
        {
            MenuOff();
        }
    }


#if !MOBILE_INPUT
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) && !StereonetDashboard.singleton.isActiveAndEnabled)
		{
            OnMenuStatusChange();
        }
    }
#endif

}
