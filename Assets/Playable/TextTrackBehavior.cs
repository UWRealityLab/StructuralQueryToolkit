using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using System.Text;
using UnityEngine.Assertions;

public class TextTrackBehavior : PlayableBehaviour 
{
    [TextArea(15, 20)]
    public string text;

    public Color color;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

        TextMeshProUGUI text = playerData as TextMeshProUGUI;


        //text.text = this.text;
        text.color = new Color(0.2f, 0.2f, 0.2f, info.weight);
    }
}

