using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(WaitForInputClip))]
public class WaitForInputTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		foreach(var c in GetClips())
		{
			var inputAsset = c.asset as WaitForInputClip;
			if (inputAsset != null)
				inputAsset.NewDuration = c.end - 1e-12; // 1e-12 makes it the last frame of this clip, not the first of the next
		}

		return base.CreateTrackMixer(graph, go, inputCount);

	}
}