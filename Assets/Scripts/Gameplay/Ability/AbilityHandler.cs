using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Ability
{
    public class AbilityHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;


        private Dictionary<AbilityType, List<Ability>> currentAbilities;


        public bool IsSkillActive(AbilityType abilityType)
        {
            return currentAbilities.ContainsKey(abilityType);
        }

        public bool IsAbilityNull(AbilityType abilityType)
        {
            return currentAbilities == null || !currentAbilities.ContainsKey(abilityType);
        }

        public Ability GetAbility(AbilityType abilityType, int? id)
        {
            if (currentAbilities == null || !currentAbilities.ContainsKey(abilityType)) return null;
            
            for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
            {
                if (currentAbilities[abilityType][i].InventorySlotID == id)
                    return currentAbilities[abilityType][i];
            }

            return null;
        }
        
        public List<Ability> GetAbilities(AbilityType abilityType)
        {
            if (currentAbilities == null || !currentAbilities.ContainsKey(abilityType) ||
                currentAbilities[abilityType].Count == 0)
                return null;
            return currentAbilities[abilityType];
        }

        public DamageData DamageModifiers(DamageData damageData)
        {
            if (!IsAbilityNull(AbilityType.BarrierDamage))
            {
                var abilities = GetAbilities(AbilityType.BarrierDamage);
                for (int i = abilities.Count - 1; i >= 0; i--)
                {
                    if (abilities[i] is BarrierDamageAbility barrierDamageAbility)
                        damageData = barrierDamageAbility.DamageModify(damageData);
                }
            }

            return damageData;
        }
        
        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
        
        
        public void AddAbility(Ability ability)
        {
            currentAbilities ??= new();
            
            if (IsAbilityNull(ability.AbilityTypeID))
                currentAbilities.Add(ability.AbilityTypeID, new List<Ability>());
            
            OnUpdate += ability.Update;
            
            currentAbilities[ability.AbilityTypeID].Add(ability);
        }

        public void RemoveAbilityByID(AbilityType abilityType, int? id)
        {
            if (!IsAbilityNull(abilityType))
            {
                for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
                {
                    if (currentAbilities[abilityType][i].InventorySlotID != id) continue;
                    
                    OnUpdate -= currentAbilities[abilityType][i].Update;
                    currentAbilities[abilityType][i].Exit();
                    
                    currentAbilities[abilityType].Remove(currentAbilities[abilityType][i]);
                }
                
                if(currentAbilities[abilityType].Count == 0)
                    currentAbilities.Remove(abilityType);
            }
        }
    }
}