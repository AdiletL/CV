using System;
using UnityEngine;

namespace Gameplay.Effect
{
    public abstract class Effect : IEffect
    {
        public event Action<IEffect> OnDestroyEffect;
        
        public GameObject target { get; protected set; }

        public Effect()
        {
            
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public abstract void ClearValues();
        
        public abstract void Update();
        public abstract void LateUpdate();

        public abstract void ApplyEffect();

        public virtual void DestroyEffect()
        {
            OnDestroyEffect?.Invoke(this);
        }
    }
}