using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StereonetsController : MonoBehaviour
{
    public static StereonetsController singleton;
    [SerializeField] Camera stereonetCamera;
    [SerializeField] GameObject stereonetPrefab;
    [SerializeField] Transform stereonetsListParent;
    List<Transform> stereonets;

    [SerializeField] public Transform originTransform;
    [SerializeField] public Transform finalPlane;
    [SerializeField] public Transform finalPlaneLeftCorner;
    [SerializeField] public Transform finalPlaneRightCorner;

    public Stereonet currStereonet;

    // Very crude way of assigning IDs to stereonet
    private bool[] availableIDs;


    private void Awake()
    {
        singleton = this;
        stereonets = new List<Transform>();
        availableIDs = new bool[6];
    }

    public void SelectStereonet(int index)
    {
        if (currStereonet != null)
        {
            currStereonet.Hide();
            //currStereonet.gameObject.SetActive(false);
        }
        currStereonet = stereonets[index].GetComponent<Stereonet>();
        AssignStereonet(currStereonet);
        currStereonet.Show();
        //currStereonet.gameObject.SetActive(true);

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.SwitchStereonet(currStereonet.id);
        }
    }

    // Creates a new stereonet
    public void CreateStereonet()
    {
        Transform stereonetTrans = Instantiate(stereonetPrefab, stereonetsListParent).transform;
        stereonets.Add(stereonetTrans);
        Stereonet newStereonet = stereonetTrans.GetComponent<Stereonet>();

        // assign open ID
        for (int i = 0; i < availableIDs.Length; i++)
        {
            if (!availableIDs[i])
            {
                newStereonet.id = i;
                availableIDs[i] = true;
                break;
            }
        }

        AssignStereonet(newStereonet);

        UpdateDashboard();
    }

    // Assigns the given stereonet to the given players' compass drawing in order to work
    void AssignStereonet(Stereonet stereonet)
    {
        if (currStereonet != null)
        {
            //currStereonet.gameObject.SetActive(false);
            currStereonet.Hide();
        }
        PolePlotting.instance.stereonet = stereonet;
        currStereonet = stereonet;

        

        if (PIPlotButton.instance)
        {
            if (currStereonet.GetNumPoints() < 3)
            {
                PIPlotButton.instance.isToggled = false;
                currStereonet.isPiPlotEnabled = false;
                currStereonet.SetLineRenderer(false);
            }
            else
            {
                PIPlotButton.instance.isToggled = currStereonet.isPiPlotEnabled;
                currStereonet.lineRenderer.enabled = currStereonet.isPiPlotEnabled;
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
        Transform stereonet = stereonets[index];
        stereonets.Remove(stereonet);
        Destroy(stereonet.gameObject);
        availableIDs[stereonet.GetComponent<Stereonet>().id] = false;

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.Delete(index);
        }
    }

    public void RemoveAll()
    {
        foreach (Transform stereonet in stereonets)
        {
            Destroy(stereonet.gameObject);
        }

        for (int i = 0; i < availableIDs.Length; i++)
        {
            availableIDs[i] = false;
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

    public void UpdateDashboard()
    {
        StartCoroutine(UpdateDashboardCoroutine());
    }


    [SerializeField] Material poleBeddingMaterial;
    [SerializeField] RawImage stereonetPlot;
    // Updates the dashboard with the latest image of the stereonet
    // Also updates the dashboard to contain the pole count of the current stereonet
    IEnumerator UpdateDashboardCoroutine()
    {
        // Converts the latest point (which is currently a unique color) to be
        // the same material as the other points
        currStereonet.SetPoleMeasurementAsStale();

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

        StereonetDashboard.singleton.UpdateCard(stereonets.IndexOf(currStereonet.transform), currStereonet.GetNumPoints(), stereonetTexture);
    }

    public Stereonet GetStereonet(int index)
    {
        return stereonets[index].GetComponent<Stereonet>();
    }



}
