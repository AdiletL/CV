using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Ability
{
    public class AbilityHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;


        private Dictionary<AbilityType, List<Ability>> currentAbilities;

        public bool IsSkillActive(AbilityType abilityType)
        {
            return currentAbilities.ContainsKey(abilityType);
        }

        public bool IsAbilityNull(AbilityType abilityType)
        {
            return currentAbilities == null 
                   || !currentAbilities.ContainsKey(abilityType) 
                   || currentAbilities[abilityType].Count == 0;
        }

        public Ability GetAbility(AbilityType abilityType, int? id)
        {
            if (IsAbilityNull(abilityType)) return null;
            
            for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
            {
                if (currentAbilities[abilityType][i].InventorySlotID == id)
                    return currentAbilities[abilityType][i];
            }

            return null;
        }
        
        public List<Ability> GetAbilities(AbilityType abilityType)
        {
            if (IsAbilityNull(abilityType))
                return null;
            return currentAbilities[abilityType];
        }

        public DamageData DamageResistanceModifiers(DamageData damageData)
        {
            var abilities = GetAbilities(AbilityType.BarrierDamage);
            if (abilities != null)
            {
                foreach (BarrierDamageAbility VARIABLE in abilities)
                {
                    damageData = VARIABLE.DamageModify(damageData);
                }
            }

            return damageData;
        }

        public List<float> GetCriticalDamages(float baseDamage)
        {
            if (currentAbilities == null) return null;
            
            var list = new List<float>();
            foreach (var lists in currentAbilities.Values)
            {
                foreach (var VARIABLE in lists)
                {
                    if (VARIABLE is ICriticalDamageApplier criticalDamageApplier)
                    {
                        if(!criticalDamageApplier.CriticalDamage.TryApply()) continue;
                        var result = criticalDamageApplier.CriticalDamage.GetCalculateDamage(baseDamage);
                        list.Add(result);
                    }
                }
            }
            return list;
        }
        
        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        
        public void AddAbility(Ability ability)
        {
            currentAbilities ??= new();
            
            if (IsAbilityNull(ability.AbilityTypeID))
                currentAbilities.Add(ability.AbilityTypeID, new List<Ability>());
            
            currentAbilities[ability.AbilityTypeID].Add(ability);
            OnUpdate += ability.Update;
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