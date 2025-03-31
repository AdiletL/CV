using UnityEngine;

namespace Gameplay.Effect
{
    public class EvasionEffect : Effect, IEvasion
    {
        public override EffectType EffectTypeID { get; } = EffectType.Evasion;
        
        public float EvasionChance { get; }

        public EvasionEffect(EvasionConfig effectConfig) : base(effectConfig)
        {
            EvasionChance = effectConfig.EvasionChance;
            IsBuff = true;
        }


        public bool TryEvade()
        {
            return Random.value < EvasionChance;
        }

        public override void ClearValues()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateEffect()
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }

    public class EvasionConfig : EffectConfig
    {
        [Range(0, 1f), Tooltip("percent")]
        public float EvasionChance;
    }
}