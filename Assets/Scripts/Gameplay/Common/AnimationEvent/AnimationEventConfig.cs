using UnityEngine;

namespace Unit.AnimationEvent
{
    public class AnimationEventConfig
    {
        public AnimationClip Clip;
        [Range(0f, 1f), Tooltip("Percent")]
        public float MomentEvent;
    }
}