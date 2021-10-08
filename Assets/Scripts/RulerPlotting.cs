using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class RulerPlotting : PlayerTool
{
    public static RulerPlotting instance;
    
    public GameObject rulerPointPrefab;
    [SerializeField] GameObject rulerLinePrefab;
    [SerializeField] AudioSource flagPlantAudio;
    [SerializeField] ChangeColorButton rulerButton;

    private Stack<RulerMeasurement> rulers;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI numRulerText;

    private float TextSize = 14f;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rulers = new Stack<RulerMeasurement>();
    }

    public override void Toggle()
    {
        base.Toggle();
        
        rulerButton.SetButtonState(isToggled);
    }

    public override void Enable()
    {
        base.Enable();

        isToggled = true;
        rulerButton.SetButtonState(true);
    }

    public override void Disable()
    {
        base.Disable();
        
        isToggled = false;
        rulerButton.SetButtonState(false);
    }

    
    public void NewRuler()
    {
        var line = Instantiate(rulerLinePrefab).GetComponent<RulerMeasurement>();
        line.SetSize(TextSize * Settings.instance.ObjectScaleMultiplier);
        rulers.Push(line);
        numRulerText.text = rulers.Count.ToString();
    }

    public override void UseTool(RaycastHit hit)
    {
        if (hit.transform.tag.Equals("Terrain"))
        {
            if (flagPlantAudio)
            {
                flagPlantAudio.Play();
            }

            // Edge case, where there are no rulers
            if (rulers.Count == 0)
            {
                NewRuler();
            }

            var ruler = rulers.Peek();
            ruler.AddPoint(hit.point, hit.normal);
        }
    }

    public override void Undo()
    {
        if (rulers.Count > 0)
        {
            rulers.Peek().Undo();
        }
    }

    public void RemoveRuler()
    {
        if (rulers.Count > 0)
        {
            Destroy(rulers.Pop().gameObject);
            numRulerText.text = rulers.Count.ToString();
        }
    }
}
