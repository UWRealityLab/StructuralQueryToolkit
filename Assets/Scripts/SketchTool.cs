using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine.UI;


public class SketchTool : PlayerTool
{
    public static SketchTool instance;
    public AnimationCurve ThicknessScaleCurve;

    // The current thicknesses that new lines will have
    private float DefaultLineThickness;

    private Color CurrLineColor = new Color(0.3f, 0.3f, 0.3f);

    [SerializeField] private GameObject optionsWindow;
    
    [SerializeField] GameObject linePrefab;
    [SerializeField] Slider thicknessSlider;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI thicknessSliderText;

    private bool isDrawing = false;
    private bool isFPSView = true;

    private LinkedList<SketchMeasurement> lines;

    private DrawMode drawMode;
    private enum DrawMode
    {
        Drawing,
        Erasing
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        lines = new LinkedList<SketchMeasurement>();
        
        SetLineThickness(thicknessSlider);

        thicknessSlider.onValueChanged.AddListener((float x) =>
        {
            SetLineThickness(thicknessSlider);
            thicknessSliderText.text = $"{DefaultLineThickness:0.###}m";
        });
    }

    public override void Toggle()
    {
        base.Toggle();
        optionsWindow.SetActive(isToggled);
    }

    public override void Enable()
    {
        base.Enable();
        optionsWindow.SetActive(true);
    }

    public override void Disable()
    {
        base.Disable();
        optionsWindow.SetActive(false);
    }

    public override void UseTool(RaycastHit hit)
    {
        if (drawMode == DrawMode.Drawing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDrawing = true;
                NewLine();
            }
            else if (isDrawing && Input.GetMouseButton(0))
            {
                var line = lines.Last.Value;
                line.AddPoint(hit.point, hit.normal);
            } 
            else if (isDrawing && Input.GetMouseButtonUp(0))
            {
                var line = lines.Last.Value;
                line.FinishLine(GameController.CurrentCamera);
                isDrawing = false;
            }
        }
        else if (drawMode == DrawMode.Erasing)
        {
            if (Input.GetMouseButton(0))
            {
                Erase();
            }
        }
    }

    public override void OnInvalidHit()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
        if (isDrawing)
        {
            var line = lines.Last.Value;
            line.FinishLine(GameController.CurrentCamera);
        }
    }

    public void NewLine()
    {
        var line = Instantiate(linePrefab);
        var pencilMeasurement = line.GetComponent<SketchMeasurement>();
        var lineRenderer = pencilMeasurement.lineRenderer;
        var currThickness = DefaultLineThickness;

        lineRenderer.startWidth = currThickness;
        lineRenderer.endWidth = currThickness;
        
        lineRenderer.startColor = CurrLineColor;
        lineRenderer.endColor = CurrLineColor;
        
        lines.AddLast(pencilMeasurement);
    }

    public void Erase()
    {
        var activeCamera = GameController.CurrentCamera;
        Ray cameraRay = activeCamera.ScreenPointToRay(Input.mousePosition);

        //var hits = Physics.SphereCastAll(cameraRay, 1f * Settings.instance.ObjectScaleMultiplier);
        var hits = Physics.RaycastAll(cameraRay);

        foreach (var hit in hits)
        {
            if (hit.transform.tag.Equals("Sketch Line"))
            {
                lines.Remove(hit.transform.GetComponent<SketchMeasurement>());
                Destroy(hit.transform.gameObject);
            }
        }
    }

    public override void Undo()
    {
        if (lines.Count > 0)
        {
            var line = lines.Last.Value;
            Destroy(line.gameObject);
            lines.RemoveLast();
        }
    }

    public void SetLineThickness(Slider slider)
    {
        DefaultLineThickness = ThicknessScaleCurve.Evaluate(slider.value) * (Settings.instance ? Settings.instance.ObjectScaleMultiplier: 1f);
    }
    
    public void SetLineColor(Image img)
    {
        CurrLineColor = img.color;
    }

    public void Hide()
    {
        foreach (var line in lines)
        {
            line.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        foreach (var line in lines)
        {
            line.gameObject.SetActive(true);
        }
    }

    public void ToggleEraserButton()
    {
        if (drawMode == DrawMode.Erasing)
        {
            drawMode = DrawMode.Drawing;
        }
        else
        {
            drawMode = DrawMode.Erasing;
        }
    }

}
