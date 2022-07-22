using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRRayInteractor))]
public class TeleportRay : MonoBehaviour
{
    [SerializeField] TeleportationProvider provider = null;

    private XRRayInteractor _rayInteractor;

    public bool _isToggled = true;

    void Awake()
    {
        _rayInteractor = GetComponent<XRRayInteractor>();
    }

    public void Toggle()
    {
        _isToggled = !_isToggled;
    }

    public void SetState(bool state)
    {
        _isToggled = state;
    }
 
    public void StartTeleportRay()
    {
        _rayInteractor.enabled = _isToggled;
    }

    public void EndTeleportRay()
    {
        _rayInteractor.enabled = false;
    }

    public void TryTeleport()
    {
        if (_isToggled && _rayInteractor.TryGetCurrent3DRaycastHit(out var hit) && hit.transform.GetComponent<TeleportationArea>())
        {
            var request = new TeleportRequest()
            {
                destinationPosition = hit.point,
                destinationRotation = Quaternion.Euler(hit.transform.forward),
                matchOrientation = MatchOrientation.WorldSpaceUp,
                requestTime = Time.time
            };
            
            provider.QueueTeleportRequest(request);
        }
    }
}
