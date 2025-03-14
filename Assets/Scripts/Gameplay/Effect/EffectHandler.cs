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
        public List<Effect> GetEffects(EffectType effectType, string id)
        {
            if (currentEffects == null || !currentEffects.ContainsKey(effectType) ||
                currentEffects[effectType].Count == 0) return null;
            var effects = new List<Effect>();

            for (int i = currentEffects[effectType].Count - 1; i >= 0; i--)
            {
                if(string.Equals(currentEffects[effectType][i].ID, id))
                    effects.Add(currentEffects[effectType][i]);
            }
            return effects;
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
            effect.OnDestroyEffect += RemoveEffects;
            effect.ApplyEffect();
        }

        public void RemoveEffects(EffectType effectType, string id)
        {
            var effects = GetEffects(effectType, id);
            if (effects == null || effects.Count == 0) return;

            for (int i = effects.Count - 1; i >= 0; i--)
            {
                OnUpdate -= effects[i].Update;
                effects[i].OnDestroyEffect -= RemoveEffects;
                effects[i].DestroyEffect();
                
                currentEffects[effectType].Remove(effects[i]);
            }
            if(currentEffects[effectType].Count == 0)
                currentEffects.Remove(effectType);
        }
    }
}