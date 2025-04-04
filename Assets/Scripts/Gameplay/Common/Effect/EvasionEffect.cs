using UnityEngine;

namespace Gameplay.Effect
{
    public class EvasionEffect : Effect
    {
        public override EffectType EffectTypeID { get; } = EffectType.Evasion;
        
        public Evasion Evasion { get; } = new Evasion();

        public EvasionEffect(EvasionConfig effectConfig) : base(effectConfig)
        {
            Evasion.EvasionStat.AddCurrentValue(effectConfig.EvasionChance);
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