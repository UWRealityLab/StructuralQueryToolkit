using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomRaycastOutlineUI : MonoBehaviour
{
    [Tooltip("If enabled, raycasts will now account for an alpha threshold to " +
    "to if the cursor is pointing at a valid button spot. Do not enable this" +
    "if your source image is not read/write enabled in the import settings")]
    public bool isNonRectangular;

    [Range(0f, 1f), DrawIf("isNonRectangular", true, DrawIfAttribute.DisablingType.ReadOnly)]
    public float alphaHitMinimumThreshold = 1f;

    private Image buttonImg;

    // Start is called before the first frame update
    void Start()
    {
        buttonImg = GetComponent<Image>();

        if (isNonRectangular)
        {
            buttonImg.alphaHitTestMinimumThreshold = alphaHitMinimumThreshold;
        }
    }
}
