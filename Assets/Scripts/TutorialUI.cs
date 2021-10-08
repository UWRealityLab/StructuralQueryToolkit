using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI instance;
    public Image backward;
    public Image forward;

    [SerializeField] TextMeshProUGUI titleText;

    [SerializeField] Color EnabledSeekButtonColor;
    [SerializeField] Color DisabledSeekButtonColor;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void SetCanSeekForward(bool canSeek)
    {
        if (canSeek)
        {
            forward.color = EnabledSeekButtonColor;
        } else
        {
            forward.color = DisabledSeekButtonColor;
        }
    }

    public void SetCanSeekBackward(bool canSeek)
    {
        if (canSeek)
        {
            backward.color = EnabledSeekButtonColor;
        } else
        {
            backward.color = DisabledSeekButtonColor;
        }
    }

    public void DisableSeekButtons()
    {
        forward.color = DisabledSeekButtonColor;
        backward.color = DisabledSeekButtonColor;
    }

    public void ResetTitleText() {
        titleText.text = "Tutorial";
        SetTitleGlow(false);
    }


    public void SetTitleText(string text) {
        titleText.text = text;
    }

    public void SetTitleGlow(bool isGlowing) {
        titleText.GetComponent<Animator>().SetBool("isGlowing", isGlowing);
    }
}
