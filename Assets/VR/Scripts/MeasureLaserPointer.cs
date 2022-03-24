using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureLaserPointer : MonoBehaviour
{
    private LineRenderer line;

    private int terrainLayer;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        terrainLayer = LayerMask.NameToLayer("terrain");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged) {
            transform.hasChanged = false;
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, transform.forward, out hit, 500f, terrainLayer)) {
                line.positionCount = 0;
                return;
            }

            line.positionCount = 2;
            line.SetPosition(1, transform.InverseTransformPoint(hit.point));

            float strike = 0, dip = 0;
            StereonetUtils.CalculateStrikeAndDip(-hit.normal, out strike, out dip);
            LatestMeasurementUI.instance.SetStrikeDipInformation(strike, dip, hit.point.y);

        }
    }
}
