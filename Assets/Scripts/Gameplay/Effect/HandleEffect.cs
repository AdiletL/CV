using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Effect
{
    public class HandleEffect : MonoBehaviour
    {
        private List<IEffect> currentEffects = new();

        public void Initialize()
        {
            
        }
        
        private void Update()
        {
            for (int i = currentEffects.Count - 1; i >= 0; i--)
            {
                currentEffects[i].UpdateEffect();
            }
        }

        public void AddEffect(IEffect effect)
        {
            if(!currentEffects.Contains(effect))
                currentEffects.Add(effect);
        }

        public void RemoveEffect(IEffect effect)
        {
            if(currentEffects.Contains(effect))
                currentEffects.Remove(effect);
        }
    }
}