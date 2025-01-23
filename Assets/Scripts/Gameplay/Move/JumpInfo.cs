using UnityEngine;
using UnityEngine.Serialization;

namespace Movement
{
    [System.Serializable]
    public class JumpInfo
    {
        public AnimationClip Clip;
        public float Height = 1.5f;
        public int MaxCount = 1;
        [FormerlySerializedAs("DecreaseEndurance")] public float ReductionEndurance = .1f;
    }
}