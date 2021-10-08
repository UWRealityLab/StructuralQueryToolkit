using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ToggleButton : MonoBehaviour
{
    [SerializeField] private bool isToggled = false;

    [SerializeField] private Sprite unToggledImaged;
    [SerializeField] private Sprite toggledImage;
    
    private Image img;
    
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    public void Toggle()
    {
        isToggled = !isToggled;
        img.sprite = isToggled ? toggledImage : unToggledImaged;
    }


}
