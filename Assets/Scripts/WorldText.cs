using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class WorldTextGroup
{
    private LinkedList<WorldText> _texts;
    
    private bool _isVisible = true;

    public WorldTextGroup()
    {
        _texts = new LinkedList<WorldText>();

    }

    public void AddText(GameObject textPrefab, Vector3 pos, float fontsize, Color color)
    {
        if (!_isVisible)
        {
            SetVisibilityState(true);
        }
        
        var newText = GameObject.Instantiate(textPrefab, pos, quaternion.identity).transform;
        newText.localScale = new float3(Settings.instance.ObjectScaleMultiplier);
        var textComponent = newText.GetComponent<WorldText>();
        textComponent.SetFontSize(fontsize);
        textComponent.ChangeColor(color);
        textComponent.group = this;
        
        //textComponent.StartEdit();
        
        _texts.AddFirst(textComponent);
    }

    
    public void ChangeColor(Color color)
    {
        foreach (var worldText in _texts)
        {
            worldText.ChangeColor(color);
        }
    }
    
    public void ChangeFontSize(float fontSize)
    {
        foreach (var worldText in _texts)
        {
            worldText.SetFontSize(fontSize);
        }
    }

    public bool GetVisibilityState()
    {
        return _isVisible;
    }

    public void SetVisibilityState(bool state)
    {
        _isVisible = state;
        if (state)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    public void Show()
    {
        foreach (var worldText in _texts)
        {
            worldText.Show();
        }
    }
    
    public void Hide()
    {
        foreach (var worldText in _texts)
        {
            worldText.Hide();
        }
    }

    public void GlobalShow()
    {
        if (_isVisible)
        {
            foreach (var worldText in _texts)
            {
                worldText.Show();
            }
        }
    }

    public void GlobalHide()
    {
        if (_isVisible)
        {
            foreach (var worldText in _texts)
            {
                worldText.Hide();
            }
        }
    }

    public void Destroy()
    {
        foreach (var worldText in _texts)
        {
            GameObject.Destroy(worldText.gameObject);
        }
        
        _texts.Clear();
    }

    public void DeleteText(WorldText text)
    {
        GameObject.Destroy(text.gameObject);
        _texts.Remove(text);
    }

    public void Undo()
    {
        if (_texts.Count <= 0)
        {
            return;
        }
        var text = _texts.First.Value;
        GameObject.Destroy(text.gameObject);
        _texts.RemoveFirst();
    }
    
    public void ShowControls()
    {
        foreach (var worldText in _texts)
        {
            worldText.ShowControls();
        }
    }

    public void HideControls()
    {
        foreach (var worldText in _texts)
        {
            worldText.HideControls();
        }
    }

    public WorldText GetLatestText()
    {
        return _texts.First.Value;
    }
}

public class WorldText : MonoBehaviour
{
    public TMP_InputField Text;
    public TMP_Text PlaceholderText;

    [SerializeField] private Canvas canvas;
    [SerializeField] private FacePlayer facePlayer;
    [SerializeField] private GameObject buttonsContainer;

    [SerializeField] private Image[] optionImages;

    public WorldTextGroup group;

    private void Start()
    {
        canvas.transform.LookAt(GameController.instance.playerObj.transform.position);
        canvas.transform.forward = -canvas.transform.forward;
    }

    public void StartEdit()
    {
        print("START EDIT");
        Text.ActivateInputField();
        TextTool.instance.EnterTextEditingMode();
        GameController.instance.DisablePlayer();
    }

    public void EndEdit()
    {
        print("END EDIT");
        TextTool.instance.LeaveTextEditingMode();
        GameController.instance.EnablePlayer();
    }

    public void SetFontSize(float fontSize)
    {
        Text.textComponent.fontSize = fontSize;
        PlaceholderText.fontSize = fontSize;
    }

    public void ChangeColor(Color color)
    {
        Text.textComponent.color = color;
        PlaceholderText.color = color;

        foreach (var img in optionImages)
        {
            img.color = color;
        }
    }
    
    public void IncrementFontSize(int amount)
    {
        Text.textComponent.fontSize += amount;
    }

    public void Rename(string text)
    {
        Text.text = text;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    public void ToggleVisibility()
    {
        if (Text.gameObject.activeSelf)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void SetCanvasWorldCamera(Camera cam)
    {
        canvas.worldCamera = cam;
    }

    public void FaceUp()
    {
        canvas.transform.rotation = Quaternion.Euler(-90f, 180f, 0f);
    }

    public void FaceTowards(Transform trans)
    {
        facePlayer.ChangePlayer(trans);
    }

    public void ToggleFaceTowardsCamera()
    {
        facePlayer.enabled = !facePlayer.enabled;
    }

    public void Delete()
    {
        group.DeleteText(this);
    }

    public void ShowControls()
    {
        buttonsContainer.SetActive(true);
    }

    public void HideControls()
    {
        buttonsContainer.SetActive(false);
    }

}
