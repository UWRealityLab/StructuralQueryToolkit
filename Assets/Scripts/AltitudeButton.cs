using UnityEngine;
using UnityEngine.EventSystems;

public class AltitudeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] bool isUp;
    [SerializeField] KeyCode key;

    private bool isDown;

    private void Update()
    {
        if (Input.GetKey(key) || isDown)
        {
            JetpackController.instance.Move(isUp ? Settings.instance.JetpackVerticalSpeed : -Settings.instance.JetpackVerticalSpeed);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
    }
}
