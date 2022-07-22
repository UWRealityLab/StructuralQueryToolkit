using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

public class RulerPlotting : PlayerTool
{
    public static RulerPlotting instance;
    public static RulerPlotting Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RulerPlotting>();
            }

            return instance;
        }
    }
    
    public GameObject rulerPointPrefab;
    [SerializeField] GameObject rulerLinePrefab;
    
    private List<RulerMeasurement> rulers;
    private RulerMeasurement _activeRuler;

    private bool _globalVisibilityState = false;

    [Header("UI")] 
    [SerializeField] private Slider fontSizeSlider;
    [SerializeField] private TMP_Text distanceFromLastSampleText; 
    
    [Header("Preview")]
    [SerializeField] private LineRenderer previewLineRenderer;
    [SerializeField] private Transform previewTextCanvas;
    [SerializeField] private TMP_Text previewText;

    private const float TEXT_SIZE = 14f;
    private int _sigFig = 2;

    protected override void Start()
    {
        base.Start();
        if (!Application.isPlaying)
        {
            return;
        }
        
        instance = this;
        if (rulers == null)
        {
            rulers = new List<RulerMeasurement>();
        }

        previewText.transform.localScale = new float3(Settings.instance.ObjectScaleMultiplier);
        fontSizeSlider.onValueChanged.AddListener((x) =>
        {
            SetFontSize(x);
        });
    }

    public void NewRuler()
    {
        var ruler = Instantiate(rulerLinePrefab).GetComponent<RulerMeasurement>();
        ruler.SetSize(TEXT_SIZE * Settings.instance.ObjectScaleMultiplier);
        if (rulers == null)
        {
            rulers = new List<RulerMeasurement>();
        }
        rulers.Add(ruler);
        _activeRuler = ruler;
    }

    private void Update()
    {
        if (GameController.instance.IsVR || !isToggled || CannotUseTool())
        {
            return;
        }
        
        if (rulers.Count > 0 && _activeRuler.GetNumPoints() > 0)
        {
            var cameraRay = GameController.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            // Instantiate a flag for one flag at the actual mouse position raycast
            if (!Physics.Raycast(cameraRay, out RaycastHit hit) || hit.transform.tag != "Terrain")
            {
                ClearPreview();
                return;
            }
            var latestPoint = _activeRuler.GetLatestPoint();
            
            previewLineRenderer.positionCount = 2;
            previewLineRenderer.SetPosition(0, latestPoint);
            previewLineRenderer.SetPosition(1, hit.point);
            previewTextCanvas.position = hit.point;
            var previewDist = Vector3.Distance(hit.point, _activeRuler.GetLatestPoint());
            previewText.text = $"{previewDist:F2}m";
        }
        else
        {
            ClearPreview();
        }
    }

    public override void Disable()
    {
        base.Disable();
        ClearPreview();
    }

    public override void UseTool(RaycastHit hit)
    {
        if (hit.transform.tag.Equals("Terrain"))
        {
            // Edge case, where there are no rulers
            if (rulers.Count == 0)
            {
                NewRuler();
            }
            
            if (_activeRuler.GetNumPoints() > 0)
            {
                var distFromLastSample = Vector3.Distance(hit.point, _activeRuler.GetLatestPoint());
                UpdateDistanceFromLastSample(distFromLastSample);
            }
            
            _activeRuler.AddPoint(hit.point, hit.normal);
        }
    }

    private void UpdateDistanceFromLastSample(float distFromLastSample)
    {
        distanceFromLastSampleText.SetText(distFromLastSample.ToString($"F{_sigFig}") + "m");
    }

    public override void Undo()
    {
        if (rulers.Count > 0)
        {
            _activeRuler.Undo();
        }
    }

    public void RemoveRuler()
    {
        if (rulers.Count > 0)
        {
            Destroy(_activeRuler.gameObject);
            _activeRuler = null;
        }
    }

    public void HideAll()
    {
        
        foreach (var ruler in rulers)
        {
            ruler.SetVisibilityState(false);
        }
    }

    public void ShowAll()
    {
        foreach (var ruler in rulers)
        {
            ruler.SetVisibilityState(true);
        }

    }

    public void SetRulerVisibility(int index, bool state)
    {
        rulers[index].SetVisibilityState(state);
    }

    public void SetGlobalVisibility(bool state)
    {
        _globalVisibilityState = state;

        if (_globalVisibilityState)
        {
            ShowAll();
        }
        else
        {
            HideAll();
        }
    }

    public void SetFontSize(float sliderVal)
    {
        foreach (var ruler in rulers)
        {
            ruler.SetSize(TEXT_SIZE * sliderVal * Settings.instance.ObjectScaleMultiplier);
        }
    }

    public void SetFontColor(Image img)
    {
        foreach (var ruler in rulers)
        {
            ruler.SetColor(img.color);
        }
    }

    public void SetSigFigs(TMP_InputField text)
    {
        if (string.IsNullOrEmpty(text.text))
        {
            text.text = $"{_sigFig}";
            return;
        }
        
        var isValid = int.TryParse(text.text, out var sigFigValue);
        
        if (!isValid)
        {
            text.text = $"{_sigFig}";
            return;
        }
        
        _sigFig = sigFigValue;
    }

    private void ClearPreview()
    {
        previewLineRenderer.positionCount = 0;
        previewText.text = "";
    }

    public void SelectRuler(int index)
    {
        _activeRuler = rulers[index];
    }

    public void DeleteSelectedRuler()
    {
        _activeRuler.Destroy();
        Destroy(_activeRuler.gameObject);
        rulers.Remove(_activeRuler);
    }

    public void DeleteAll()
    {
        foreach (var ruler in rulers)
        {
            ruler.Destroy();
        }
        rulers.Clear();
        _activeRuler = null;
    }
}
