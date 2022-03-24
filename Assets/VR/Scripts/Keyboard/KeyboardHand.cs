using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class KeyboardHand : MonoBehaviour
{
    public UnityEvent measureAction;

    public KeyboardKey hoveredKey;

    public LineRenderer line;
    private int keyboardLayer;
    private int keyboardBaseLayer;
    private int UIlayer;
    public Hand hand;

    // Start is called before the first frame update
    void Awake()
    {
        hand = GetComponentInChildren<Hand>();
        keyboardLayer = LayerMask.NameToLayer("Keyboard");
        keyboardBaseLayer = LayerMask.NameToLayer("Keyboard Base");
        UIlayer = LayerMask.NameToLayer("UI");
    }

    private void OnDisable() {
        line.enabled = false;
        if (hoveredKey != null) {
            hoveredKey.EndHover();
        }
        //measureAction.RemoveOnChangeListener(Type, hand.handType);
    }

    private void OnEnable() {
        line.enabled = true;
        //measureAction.AddOnChangeListener(Type, hand.handType);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(line.transform.position, line.transform.forward, 20f, ~keyboardLayer | ~keyboardBaseLayer);

        if (hits.Length == 0) {
            line.SetPosition(1, Vector3.zero);
            if (hoveredKey != null) {
                hoveredKey.EndHover();
                hoveredKey = null;
            }
            return;
        }
 
        foreach (var hit in hits)
        {

            if (hit.transform.gameObject.layer == keyboardLayer)
            {
                line.SetPosition(1, line.transform.InverseTransformPoint(hit.point));

                if (hoveredKey != null)
                {
                    hoveredKey.EndHover();
                }

                hoveredKey = hit.transform.GetComponent<KeyboardKey>();
                hoveredKey.Hover();
            }
            else if (hit.transform.gameObject.layer == keyboardBaseLayer)
            {
                line.SetPosition(1, line.transform.InverseTransformPoint(hit.point));
                if (hoveredKey != null)
                {
                    hoveredKey.EndHover();
                }
            }
        }
    }
}
