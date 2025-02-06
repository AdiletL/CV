using UnityEngine;
using UnityEngine.Serialization;

namespace Movement
{
    [System.Serializable]
    public class JumpInfo
    {
        public AnimationClip Clip;
        public float Power = 1.5f;
        public int MaxCount = 1;
        public float BaseReductionEndurance = .1f;
    }
}