using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class TextTool : PlayerTool
{
    public static TextTool instance;
    public static TextTool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TextTool>();
            }

            return instance;
        }
    }

    [SerializeField] private GameObject textPrefab;
    [SerializeField] private SegmentedControlButton visibilityButton;
    
    private List<WorldTextGroup> _textGroups;

    private WorldTextGroup _activeTextGroup;

    private const float TEXT_SIZE = 10f;

    private bool _globalVisibility = true;
    private Color _currColor;
    private float _currFontSize;
    private bool _isTyping = false;

    private void Awake()
    {
        instance = this;

        _textGroups = new List<WorldTextGroup>();
        _currColor = Color.white;
    }

    protected override void Start()
    {
        base.Start();
        
        if (Application.isPlaying)
        {
            _currFontSize = TEXT_SIZE;
        }

        /*if (GameController.instance)
        {
            //GameController.instance.switchToMapViewEvent.AddListener(FaceMapCamera);
            //GameController.instance.returnToFPSEvent.AddListener(FacePlayerCamera);
        }
        else
        {
            Debug.LogError("GameController not set");
        }*/
    }

    public override void Enable()
    {
        base.Enable();
        ShowWorldButtonControls();
    }

    public override void Disable()
    {
        base.Disable();
        HideWorldButtonControls();
    }
    
    public override void UseTool(RaycastHit hit)
    {
        if (_isTyping)
        {
            return;
        }

        if (!_globalVisibility)
        {
            // User has all texts set to invisible, so make them visible again
            ShowAllWorldText();
            visibilityButton.SetState(true);
        }
        
        _activeTextGroup.AddText(textPrefab, hit.point, _currFontSize, _currColor);
    }
    
    public override void Undo()
    {
        _activeTextGroup.Undo();
    }

    public void SelectTextGroup(int index)
    {
        _activeTextGroup = _textGroups[index];
    }

    public void DeleteTextGroup(int index)
    {
        _textGroups[index].Destroy();
        _textGroups.RemoveAt(index);
    }
    
    public void DeleteActiveTextGroup()
    {
        _activeTextGroup.Destroy();
        _textGroups.Remove(_activeTextGroup);
    }

    public void DeleteAll()
    {
        foreach (var textGroup in _textGroups)
        {
            textGroup.Destroy();
        }

        _textGroups.Clear();
    }
    
    public void CreateNewTextGroup()
    {
        if (_textGroups == null)
        {
            _textGroups = new List<WorldTextGroup>();
        }
        _textGroups.Add(new WorldTextGroup());
    }

    /// <summary>
    /// Changes the color of every world text
    /// </summary>
    public void ChangeTextColor(Image colorImg)
    {
        var color = colorImg.color;
        _currColor = color;
        foreach (var worldTextGroup in _textGroups)
        {
            worldTextGroup.ChangeColor(color);
        }
    }

    /// <summary>
    /// Changes font size of every world text
    /// </summary>
    public void ChangeFontSize(Slider slider)
    {
        var fontSize = slider.value * TEXT_SIZE;
        _currFontSize = fontSize;
        foreach (var worldTextGroup in _textGroups)
        {
            worldTextGroup.ChangeFontSize(_currFontSize);
        }
    }

    public void SetTextGroupVisibility(int groupIndex, bool state)
    {
        _textGroups[groupIndex].SetVisibilityState(state);
    }

    public void ShowAllWorldText()
    {
        _globalVisibility = true;

        foreach (var textGroup in _textGroups)
        {
            textGroup.GlobalShow();
        }
    }

    public void HideAllWorldText()
    {       
        _globalVisibility = false;

        foreach (var textGroup in _textGroups)
        {
            textGroup.GlobalHide();
        }
    }

    public bool GetVisibilityState(int index)
    {
        return _textGroups[index].GetVisibilityState();
    }

    /*
    private void FacePlayerCamera()
    {
        if (MapView.instance)
        {
            var playerTrans = GameController.instance.playerObj.transform;
            var playerCam = GameController.instance.playerCamera.GetComponent<Camera>();
            foreach (var text in _textGroups)
            {
                text.SetCanvasWorldCamera(playerCam);
                text.FaceTowards(playerTrans);
            }
        }
    }
    private void FaceMapCamera()
    {
        if (MapView.instance)
        {
            var mapViewTrans = MapView.instance.mapViewCamera.transform;
            var mapViewCam = MapView.instance.mapViewCamera;
            foreach (var text in _textGroups)
            {
                text.SetCanvasWorldCamera(mapViewCam);
                text.FaceTowards(mapViewTrans);
            }
        }
    }*/

    public void EnterTextEditingMode()
    {
        if (PauseMenu.instance)
        {
            PauseMenu.instance.enabled = false;
        }
        _isTyping = true;
    }

    public void LeaveTextEditingMode()
    {
        if (PauseMenu.instance)
        {
            PauseMenu.instance.enabled = true;
        }
        StartCoroutine(AllowTypingCoroutine());
    }

    private IEnumerator AllowTypingCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _isTyping = false;
    }

    private void HideWorldButtonControls()
    {
        foreach (var worldTextGroup in _textGroups)
        {
            worldTextGroup.HideControls();
        }
    }
    
    private void ShowWorldButtonControls()
    {
        foreach (var worldTextGroup in _textGroups)
        {
            worldTextGroup.ShowControls();
        }
    }
}
