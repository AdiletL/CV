using UnityEngine;

namespace Movement
{
    [System.Serializable]
    public class JumpInfo
    {
        public AnimationCurve Curve;
        public AnimationClip Clip;
        public float Duration = 1f;
        public float Height = 1.5f;
        public int MaxCount = 1;
    }
}