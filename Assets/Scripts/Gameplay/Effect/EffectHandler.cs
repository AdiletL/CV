using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Effect
{
    public class EffectHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        
        
        private Dictionary<EffectType, List<Effect>> currentEffects = new();
        
        public Effect GetEffect(Effect effect)
        {
            for (int i = currentEffects[effect.EffectTypeID].Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(currentEffects[effect.EffectTypeID][i], effect))
                    return currentEffects[effect.EffectTypeID][i];
            }

            return null;
        }

        public List<Effect> GetEffects(EffectType effectType)
        {
            if (currentEffects == null || !currentEffects.ContainsKey(effectType) ||
                currentEffects[effectType].Count == 0) return null;
            return currentEffects[effectType];
        }

        public void Initialize()
        {
            
        }
        
        private void Update() => OnUpdate?.Invoke();
        

        public void AddEffect(Effect effect)
        {
            if(!currentEffects.ContainsKey(effect.EffectTypeID))
                currentEffects.Add(effect.EffectTypeID, new List<Effect>());
            currentEffects[effect.EffectTypeID].Add(effect);
            
            OnUpdate += effect.Update;
            effect.OnDestroyEffect += OnDestroyEffect;
            effect.ApplyEffect();
        }

        public void OnDestroyEffect(Effect effect)
        {
            RemoveEffect(effect);
        }

        public void RemoveEffect(Effect effect)
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