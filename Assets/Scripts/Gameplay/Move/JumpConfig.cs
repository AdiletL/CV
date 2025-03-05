using Unit.Character.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Movement
{
    [System.Serializable]
    public class JumpConfig
    {
        public InputType BlockInputType;
        public AnimationClip Clip;
        public float Power = 1.5f;
        public int MaxCount = 1;
        public float BaseReductionEndurance = .1f;
    }
}