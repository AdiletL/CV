using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Effect
{
    public class HandleEffect : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        
        
        private List<IEffect> currentEffects = new();

        public bool IsEffectActive(IEffect effect)
        {
            return currentEffects.Contains(effect);
        }
        
        public void Initialize()
        {
            
        }
        
        private void Update() => OnUpdate?.Invoke();

        private void LateUpdate() => OnLateUpdate?.Invoke();
        

        public void AddEffect(IEffect effect)
        {
            if (!currentEffects.Contains(effect))
            {
                OnUpdate += effect.Update;
                OnLateUpdate += effect.LateUpdate;
                effect.OnDestroyEffect += RemoveEffect;
                currentEffects.Add(effect);
            }
        }

        public void RemoveEffect(IEffect effect)
        {
            if (currentEffects.Contains(effect))
            {
                OnUpdate -= effect.Update;
                OnLateUpdate -= effect.LateUpdate;
                effect.OnDestroyEffect -= RemoveEffect;
                currentEffects.Remove(effect);
            }
        }
    }
}