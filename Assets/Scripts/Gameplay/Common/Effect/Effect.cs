using System;
using UnityEngine;

namespace Gameplay.Effect
{
    public abstract class Effect : IEffect
    {
        public event Action<IEffect> OnDestroyEffect;

        public abstract EffectType EffectTypeID { get; }
        public EffectCategory EffectCategoryID { get; }

        public GameObject Target { get; protected set; }
        

        public Effect(EffectConfig effectConfig)
        {
            EffectCategoryID = effectConfig.EffectCategoryID;
        }

        public virtual void SetTarget(GameObject target) => this.Target = target;

        public abstract void ClearValues();
        
        public abstract void Update();
        public abstract void FixedUpdate();

        public abstract void UpdateEffect();
        public abstract void ApplyEffect();

        public virtual void RemoveEffect()
        {
            if (EffectCategoryID.HasFlag(EffectCategory.Passive)) return;
            OnDestroyEffect?.Invoke(this);
        }

        public virtual void DestroyEffect()
        {
            OnDestroyEffect?.Invoke(this);
        }
    }

    public abstract class EffectConfig
    {
        public EffectCategory EffectCategoryID;
    }
}