using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TextClip : PlayableAsset
{
    [TextArea(15, 20)]
    public string text;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {

        var playable = ScriptPlayable<TextTrackBehavior>.Create(graph);

        TextTrackBehavior textTrackBehavior = playable.GetBehaviour();
        textTrackBehavior.text = text;

        return playable;

    }
}
