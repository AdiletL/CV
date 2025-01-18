using UnityEngine;

namespace Movement
{
    [System.Serializable]
    public class JumpInfo
    {
        public AnimationClip Clip;
        public float Height = 1.5f;
        public int MaxCount = 1;
        public float DecreaseEndurance = .1f;
    }
}