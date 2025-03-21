﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    [System.Serializable]
    public class AnimationEventConfig
    {
        public AnimationClip Clip;
        [Range(0f, 1f), Tooltip("Percent")] public float[] MomentEvents;
    }
}
