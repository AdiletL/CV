using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Effect
{
    public class EffectHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        
        
        private Dictionary<EffectType, List<IEffect>> currentEffects = new();

        public bool IsEffectNull(EffectType effectType)
        {
            if (currentEffects == null 
                || !currentEffects.ContainsKey(effectType) 
                || currentEffects[effectType].Count == 0)
                return true;
            
            return false;
        }
        
        public IEffect GetEffect(IEffect effect)
        {
            for (int i = currentEffects[effect.EffectTypeID].Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(currentEffects[effect.EffectTypeID][i], effect))
                    return currentEffects[effect.EffectTypeID][i];
            }

            return null;
        }

        public List<IEffect> GetEffects(EffectType effectType)
        {
            if (IsEffectNull(effectType)) return null;
            return currentEffects[effectType];
        }

        public void Initialize()
        {
            
        }
        
        private void Update() => OnUpdate?.Invoke();
        

        public void AddEffect(IEffect effect)
        {
            if(!currentEffects.ContainsKey(effect.EffectTypeID))
                currentEffects.Add(effect.EffectTypeID, new List<IEffect>());
            currentEffects[effect.EffectTypeID].Add(effect);
            
            OnUpdate += effect.Update;
            effect.OnDestroyEffect += OnDestroyEffect;
            effect.ApplyEffect();
        }

        public void OnDestroyEffect(IEffect effect)
        {
            RemoveEffect(effect);
        }

        public void RemoveEffect(IEffect effect)
        {
            var targetEffect = GetEffect(effect);
            if (targetEffect == null) return;

            OnUpdate -= targetEffect.Update;
            targetEffect.OnDestroyEffect -= OnDestroyEffect;
            targetEffect.DestroyEffect();
            currentEffects[targetEffect.EffectTypeID].Remove(targetEffect);
            
            if(currentEffects[targetEffect.EffectTypeID].Count == 0)
                currentEffects.Remove(targetEffect.EffectTypeID);
        }
    }
}