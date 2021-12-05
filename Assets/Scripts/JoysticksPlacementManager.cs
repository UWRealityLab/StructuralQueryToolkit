using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A very hacky way to ensure that the joysticks do not overlap with the Stereonet UI when on mobile phones.
/// It accommodates for tall displays, where the joysticks should be close to the edges for usability
/// </summary>
public class JoysticksPlacementManager : MonoBehaviour
{
    [Header("Joysticks")] 
    [SerializeField] private RectTransform rightJoystick;
    [SerializeField] private RectTransform leftJoystick;
    
    [Header("Options")]
    [Tooltip("Stereonet UI height + its sub-windows heights")] public float HeightThreshold;
    public float ShortDisplayJoystickOffset;
    public float TallDisplayJoystickOffset;

    
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isMobilePlatform)
        {
            // For mobile (landscape) only
            Destroy(this.gameObject);
            return;
        }
        
        bool isTall = Screen.height >= HeightThreshold;
        
        rightJoystick.anchoredPosition = new Vector2(isTall ? TallDisplayJoystickOffset : ShortDisplayJoystickOffset, rightJoystick.anchoredPosition.y);
        leftJoystick.anchoredPosition = new Vector2(isTall ? -TallDisplayJoystickOffset : -ShortDisplayJoystickOffset, leftJoystick.anchoredPosition.y);
    }

}
