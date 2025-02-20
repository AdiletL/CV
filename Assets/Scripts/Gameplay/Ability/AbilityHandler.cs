using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Ability
{
    public class AbilityHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;


        private Dictionary<AbilityType, List<IAbility>> currentAbilities;


        public bool IsSkillActive(AbilityType abilityType)
        {
            return currentAbilities.ContainsKey(abilityType);
        }

        public bool IsAbilityNotNull(AbilityType abilityType)
        {
            return currentAbilities != null && currentAbilities.ContainsKey(abilityType);
        }

        public IAbility GetAbility(AbilityType abilityType, int? id)
        {
            if (currentAbilities == null || !currentAbilities.ContainsKey(abilityType)) return null;
            
            for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
            {
                if (currentAbilities[abilityType][i].InventorySlotID == id)
                    return currentAbilities[abilityType][i];
            }

            return null;
        }
        
        public List<IAbility> GetAbilities(AbilityType abilityType)
        {
            return currentAbilities?[abilityType];
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
        
        
        public void AddAbility(IAbility iAbility)
        {
            currentAbilities ??= new();
            
            if (!IsAbilityNotNull(iAbility.AbilityType))
                currentAbilities.Add(iAbility.AbilityType, new List<IAbility>());
            
            OnUpdate += iAbility.Update;
            OnLateUpdate += iAbility.LateUpdate;
            
            currentAbilities[iAbility.AbilityType].Add(iAbility);
        }

        public void RemoveAbilityByID(AbilityType abilityType, int? id)
        {
            if (IsAbilityNotNull(abilityType))
            {
                for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
                {
                    if (currentAbilities[abilityType][i].InventorySlotID != id) continue;
                    
                    OnUpdate -= currentAbilities[abilityType][i].Update;
                    OnLateUpdate -= currentAbilities[abilityType][i].LateUpdate;
                    currentAbilities[abilityType][i].Exit();
                    
                    currentAbilities[abilityType].Remove(currentAbilities[abilityType][i]);
                    break;
                }
            }
        }
    }
}