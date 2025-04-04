using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Movement
{
    [System.Serializable]
    public class JumpConfig
    {
        public InputType BlockInputType;
        public AnimationClip Clip;
        public float Power = 1.5f;
        public int MaxCount = 1;
        [FormerlySerializedAs("BaseReductionEndurance")] public float ConsumptionEndurance = .1f;
    }
}