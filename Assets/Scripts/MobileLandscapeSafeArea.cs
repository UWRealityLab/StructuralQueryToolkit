using UnityEngine;

public class MobileLandscapeSafeArea : MonoBehaviour
{
    private void Start() {
        ApplySafeArea();
    }

    private void ApplySafeArea() {
        Rect safeAreaRect = Screen.safeArea;

        var parentRectTransform = GetComponentInParent<RectTransform>();
        
        float scaleRatio = parentRectTransform.rect.width / Screen.width;

        var left = safeAreaRect.xMin * scaleRatio;
        var right = -(Screen.width - safeAreaRect.xMax) * scaleRatio;
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMin = new Vector2(left, 0f);
        rectTransform.offsetMax = new Vector2(right, 0f);
    }
}
