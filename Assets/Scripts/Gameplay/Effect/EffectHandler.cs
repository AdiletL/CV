using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Effect
{
    public class EffectHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        
        
        private Dictionary<EffectType, List<Effect>> currentEffects = new();
        
        public Effect GetEffect(EffectType effectType, string id)
        {
            if (currentEffects == null || !currentEffects.ContainsKey(effectType) ||
                currentEffects[effectType].Count == 0) return null;

            for (int i = currentEffects[effectType].Count - 1; i >= 0; i--)
            {
                if(string.Equals(currentEffects[effectType][i].ID, id))
                    return currentEffects[effectType][i];
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
            if (GetEffect(effect.EffectTypeID, effect.ID) != null) return;
            
            OnUpdate += effect.Update;
            effect.OnDestroyEffect += RemoveEffect;
            effect.ApplyEffect();
            
            if(!currentEffects.ContainsKey(effect.EffectTypeID))
                currentEffects.Add(effect.EffectTypeID, new List<Effect>());
            currentEffects[effect.EffectTypeID].Add(effect);
        }

        public void RemoveEffect(EffectType effectType, string id)
        {
            Effect effect = GetEffect(effectType, id);
            if (effect == null) return;
            
            OnUpdate -= effect.Update;
            effect.OnDestroyEffect -= RemoveEffect;
            effect.DestroyEffect();
            
            currentEffects[effectType].Remove(effect);
            if(currentEffects[effectType].Count == 0)
                currentEffects.Remove(effectType);
        }
    }
}