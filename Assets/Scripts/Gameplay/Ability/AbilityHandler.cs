using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Ability
{
    public class AbilityHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;


        private Dictionary<AbilityType, List<IAbility>> currentAbilities = new();


        public bool IsSkillActive(AbilityType abilityType)
        {
            return currentAbilities.ContainsKey(abilityType);
        }

        public bool IsAbilityNotNull(AbilityType abilityType)
        {
            return currentAbilities.ContainsKey(abilityType);
        }

        public IAbility GetAbility(AbilityType abilityType, int? id)
        {
            if (!currentAbilities.ContainsKey(abilityType)) return null;
            
            for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
            {
                if (currentAbilities[abilityType][i].SlotID == id)
                    return currentAbilities[abilityType][i];
            }

            return null;
        }
        
        public List<IAbility> GetSkills(AbilityType abilityType)
        {
            return currentAbilities[abilityType];
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
        

        public void Activate(AbilityType abilityType, int? id, Action exitCallback = null)
        {
            if (IsAbilityNotNull(abilityType))
            {
                for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
                {
                    if(currentAbilities[abilityType][i].SlotID != id) continue;
                    currentAbilities[abilityType][i].Activate(exitCallback);
                    break;
                }
            }
        }
        

        public void AddAbility(IAbility iAbility)
        {
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
                    if (currentAbilities[abilityType][i].SlotID != id) continue;
                    
                    OnUpdate -= currentAbilities[abilityType][i].Update;
                    OnLateUpdate -= currentAbilities[abilityType][i].LateUpdate;
                    currentAbilities[abilityType][i].Exit();
                    
                    currentAbilities[abilityType].Remove(currentAbilities[abilityType][i]);
                    break;
                }
            }
        }

        public void ExitSkillByID(AbilityType abilityType, int id)
        {
            if (IsAbilityNotNull(abilityType))
            {
                for (int i = currentAbilities[abilityType].Count - 1; i >= 0; i--)
                {
                    if (currentAbilities[abilityType][i].SlotID != id) continue;
                    currentAbilities[abilityType][i].Exit();
                    break;
                }
            }
        }
    }
}