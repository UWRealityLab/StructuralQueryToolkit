using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class ButtonSoundPlayer : MonoBehaviour, IPointerClickHandler
{
    public AudioSource source;
    public AudioClip sound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (source && sound)
        {
            source.clip = sound;
            source.Play();
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ButtonSoundPlayer))]
public class ButtonWhalebackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Unity Button to all ButtonWhaleback buttons"))
        {
            var whalebackButtons = FindObjectsOfType<ButtonSoundPlayer>(true);

            foreach (var whaleButton in whalebackButtons)
            {
                var button = whaleButton.GetComponent<Button>();
                if (button)
                {
                    button.transition = Selectable.Transition.Animation;
                    button.animationTriggers.pressedTrigger = "isPressed";
                    continue;
                }

                Button newButton = whaleButton.gameObject.AddComponent<Button>();
                newButton.transition = Selectable.Transition.Animation;
                newButton.animationTriggers.pressedTrigger = "isPressed";

            }
        }
    }
}

#endif