using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StereonetsController : MonoBehaviour
{
    public static StereonetsController instance;

    [Space(20f)]
    
    [SerializeField] Camera stereonetCamera;
    [SerializeField] GameObject stereonetPrefab;
    [SerializeField] Transform stereonetsListParent;
    public List<Stereonet> stereonets;
    
    [SerializeField] public Transform originTransform;
    [SerializeField] public Transform finalPlane;

    public Stereonet currStereonet;
    
    [Header("3D Stereonet")] 
    [SerializeField] private GameObject stereonet3DImage;

    [Header("2D Stereonet")] 
    [SerializeField] private GameObject stereonet2DPrefab;
    [SerializeField] private GameObject stereonetImage2D;

    private int _numStereonetsCreated = 0;

    public UnityEvent<Stereonet> OnStereonetCreated;

    private void Awake()
    {
        instance = this;
        stereonets = new List<Stereonet>();
        OnStereonetCreated = new UnityEvent<Stereonet>();
    }

    public void SelectStereonet(int index)
    {
        if (currStereonet != null)
        {
            currStereonet.Hide();
        }
        
        currStereonet = stereonets[index];
        AssignStereonet(currStereonet);
        currStereonet.Show();

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.SwitchStereonet(currStereonet.id);
        }
        
        StereonetCamera.instance.UpdateStereonet();
    }

    // Creates a new stereonet
    public Stereonet CreateStereonet()
    {
        _numStereonetsCreated++;
        var stereonetTrans = Instantiate(stereonet2DPrefab, stereonetsListParent).transform;
        var newStereonet = stereonetTrans.GetComponent<Stereonet>();
        newStereonet.id = _numStereonetsCreated;
        stereonets.Add(newStereonet);
        AssignStereonet(newStereonet);

        UpdateStereonetDashboard();
        
        OnStereonetCreated.Invoke(newStereonet);

        return newStereonet;
    }

    // Assigns the given stereonet to the given players' compass drawing in order to work
    void AssignStereonet(Stereonet stereonet)
    {
        if (currStereonet != null)
        {
            currStereonet.Hide();
        }
        currStereonet = stereonet;

        
        if (PIPlotButton.instance)
        {
            if (currStereonet.GetNumPoints() < 3)
            {
                PIPlotButton.instance.isToggled = false;
                currStereonet.isPiPlotEnabled = false;
                currStereonet.SetPoleLineRendererState(false);
            }
            else
            {
                PIPlotButton.instance.isToggled = currStereonet.isPiPlotEnabled;
                currStereonet.SetPoleLineRendererState(currStereonet.isPiPlotEnabled);
            }
            PIPlotButton.instance.UpdateButton();
        }

        if (PiPlotPlaneButton.instance)
        {
            PiPlotPlaneButton.instance.UpdateButton();
        }

        if (PiPlotLinearButton.instance)
        {
            PiPlotLinearButton.instance.UpdateButton();
        }

    }

    public void Delete(int index)
    {
        var stereonet = stereonets[index];
        stereonets.Remove(stereonet);
        Destroy(stereonet.gameObject);

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.Delete(index);
        }
    }

    public void RemoveAll()
    {
        _numStereonetsCreated = 0;
        foreach (var stereonet in stereonets)
        {
            Destroy(stereonet.gameObject);
        }

        stereonets.Clear();
    }

    public void Undo()
    {
        ToolManager.instance.undoEvent.Invoke();
    }

    public void UndoPole()
    {
        currStereonet.UndoPole();
    }

    public void UndoLine()
    {
        currStereonet.UndoLine();
    }

    public void UndoPlane()
    {
        currStereonet.UndoPlane();
    }

    public void UpdateStereonetDashboard()
    {
        // Converts the latest point (which is currently a unique color) to be
        // the same material as the other points
        currStereonet.SetLatestPointMeasurementAsStale();
    }
    

    [SerializeField] RawImage stereonetPlot;
    // Updates the dashboard with the latest image of the stereonet
    // Also updates the dashboard to contain the pole count of the current stereonet
    IEnumerator UpdateDashboardCoroutine()
    {
        // Wait one frame to allow the stereonet to render
        yield return new WaitForEndOfFrame();

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D stereonetTexture = new Texture2D(stereonetPlot.mainTexture.width, stereonetPlot.mainTexture.height, TextureFormat.ARGB32, 1, false);

        if (SystemInfo.copyTextureSupport == CopyTextureSupport.None)
        {
            // If CopyTexture() is not supported, then use ReadPixels() to send the GPU data to the CPU and copies it?
            // It's a lot slower (tested on Galaxy Edge 7)
            // Context: WebGL and older mobile phones do not support CopyTexture()
            //print("Using ReadPixels");

            RenderTexture currentActiveRT = RenderTexture.active;

            RenderTexture rt = stereonetCamera.targetTexture;
            // Set the supplied RenderTexture as the active one
            RenderTexture.active = rt;

            //Texture2D stereonetTexture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);

            stereonetTexture.ReadPixels(new Rect(0, 0, stereonetTexture.width, stereonetTexture.height), 0, 0);
            stereonetTexture.anisoLevel = 16;
            stereonetTexture.filterMode = FilterMode.Trilinear;

            // Restore previously active render texture
            RenderTexture.active = currentActiveRT;

            stereonetTexture.Apply();
        }
        else
        {
            // For Android devices that support OpenGL ES 3.1 (which implies that it's running Android 5.0 or higher I think)
            // CopyTexture() does the copying in the GPU, which is a lot faster
            Graphics.CopyTexture(stereonetPlot.mainTexture, stereonetTexture);
        }

        StereonetDashboard.instance.UpdateCard(stereonets.IndexOf(currStereonet), stereonetTexture);
    }

    public Stereonet GetStereonet(int index)
    {
        return stereonets[index].GetComponent<Stereonet>();
    }

    /// <summary>
    /// Toggles between showing the 3D and 2D versions of the stereonet on the stereonet card
    /// </summary>
    public void ToggleStereonetMode()
    {
        stereonet3DImage.SetActive(!stereonet3DImage.activeSelf);
        stereonetImage2D.SetActive(!stereonetImage2D.activeSelf);
    }

    public void RotateStereonet()
    {
        currStereonet.RotateModel();
        stereonetCamera.Render();
    }
    
    
}
