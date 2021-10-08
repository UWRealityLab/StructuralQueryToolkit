using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrowController : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    [SerializeField] Transform arrowTransform;
    [SerializeField] Transform targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform == null) {
            Debug.LogError("Target Transform is null");
            return;
        }

        // Max exported the arrow to have different xyz euler angles than Unity, so convert them
        Vector3 direction = targetTransform.position - playerTransform.position;
        Vector3 lookDir = new Vector3(direction.x, direction.z, direction.y);
        arrowTransform.forward = lookDir;

    }
}
