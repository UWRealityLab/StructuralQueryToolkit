using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonDrawerButton : MonoBehaviour
{
    public Image SelectedIcon;


    public void Select()
    {
        SelectedIcon.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        SelectedIcon.gameObject.SetActive(false);
    }
}
