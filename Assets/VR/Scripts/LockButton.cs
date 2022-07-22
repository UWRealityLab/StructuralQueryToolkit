using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockButton : MonoBehaviour
{
    [SerializeField] OrbitUI orbitUI;

    [SerializeField] Image lockedIcon;
    [SerializeField] Image unlockedIcon;
    [SerializeField] TextMeshProUGUI text;

    public void Toggle() {
        if (orbitUI.enabled) {
            orbitUI.enabled = false;
            lockedIcon.enabled = true;
            unlockedIcon.enabled = false;
            text.text = "UNLOCK WINDOW";
        } else {
            orbitUI.enabled = true;
            lockedIcon.enabled = false;
            unlockedIcon.enabled = true;
            text.text = "LOCK WINDOW";
        }
    }
}
