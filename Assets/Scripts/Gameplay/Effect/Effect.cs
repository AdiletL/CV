using UnityEngine;

namespace Gameplay.Effect
{
    public abstract class Effect : IEffect
    {
        public GameObject target { get; protected set; }

        public Effect()
        {
            
        }
        

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public abstract void ResetEffect();
        public abstract void UpdateEffect();

        public abstract void ApplyEffect();
        
        public abstract void DestroyEffect();
    }
}