using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class WaitForInputClip : PlayableAsset, ITimelineClipAsset
{
    public double NewDuration {get;set;}

    public class WaitForInputBehaviour : PlayableBehaviour
    {
        public double NewDuration;
        public PlayableDirector Director;

        
        double m_PreviousDuration;
        DirectorWrapMode m_PrevWrapMode;
        

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_PreviousDuration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            playable.GetGraph().GetRootPlayable(0).SetDuration(NewDuration);
            
            // we could use the playable graph, but the wrap mode method is internal. PlayableDirector.extrapolationMode will update a playing graph
            if (Director != null)
            {
                m_PrevWrapMode = Director.extrapolationMode;
                Director.extrapolationMode = DirectorWrapMode.Hold;
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (System.Math.Abs(playable.GetGraph().GetRootPlayable(0).GetDuration() - NewDuration) < 0.0001)
            {
                playable.GetGraph().GetRootPlayable(0).SetDuration(m_PreviousDuration);
                if (Director != null)
                    Director.extrapolationMode = m_PrevWrapMode;
            }
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (Input.anyKey)
            {
                playable.GetGraph().GetRootPlayable(0).SetDuration(m_PreviousDuration);
                if (Director != null)
                    Director.extrapolationMode = m_PrevWrapMode;

            }
        }
    }
	
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // input requires playmode
        if (!Application.isPlaying)
            return Playable.Create(graph);

        var scriptPlayable = ScriptPlayable<WaitForInputBehaviour>.Create(graph);
        scriptPlayable.GetBehaviour().NewDuration = NewDuration;
        scriptPlayable.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
        return scriptPlayable;
    }

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }
}
