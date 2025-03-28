using System;
using UnityEngine;

namespace Gameplay.Effect
{
    public abstract class Effect : IEffect
    {
        public event Action<Effect> OnDestroyEffect;

        public abstract EffectType EffectTypeID { get; }

        public GameObject Target { get; protected set; }
        public bool IsInterim { get; protected set; }
        public bool IsBuff { get; protected set; }
        

        public Effect(EffectConfig effectConfig)
        {
            
        }

        public void SetModifier(bool isBuff) => IsBuff = isBuff;
        public virtual void SetTarget(GameObject target) => this.Target = target;

        public abstract void ClearValues();
        
        public abstract void Update();
        public abstract void FixedUpdate();

        public abstract void UpdateEffect();
        public abstract void ApplyEffect();

        public virtual void DestroyEffect()
        {
            OnDestroyEffect?.Invoke(this);
        }
    }

    public abstract class EffectConfig
    {
        public bool IsInterim = true;
    }
}