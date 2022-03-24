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
    

    void Awake()
    {
        _rayInteractor = GetComponent<XRRayInteractor>();
    }

    public void Toggle()
    {
        _rayInteractor.enabled = !_rayInteractor.enabled;
    }

    public void Activate()
    {
        _rayInteractor.enabled = true;
    }

    public void Deactivate()
    {
        _rayInteractor.enabled = false;
    }

    public void TryTeleport()
    {
        if (_rayInteractor.TryGetCurrent3DRaycastHit(out var hit) && hit.transform.GetComponent<TeleportationArea>())
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
