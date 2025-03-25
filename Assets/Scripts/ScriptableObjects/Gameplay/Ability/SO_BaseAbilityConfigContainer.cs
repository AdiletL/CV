using System;
using UnityEngine;

namespace ScriptableObjects.Ability
{
    [CreateAssetMenu(fileName = "SO_BaseAbilityConfigContainer", menuName = "SO/Gameplay/Ability/Container", order = 51)]
    public class SO_BaseAbilityConfigContainer : ScriptableObject
    {
        [SerializeField] private SO_BaseAbilityConfig[] abilityConfigs;

        public SO_BaseAbilityConfig GetAbilityConfig(AbilityType abilityType)
        {
            for (int i = 0; i < abilityConfigs.Length; i++)
            {
                if(abilityConfigs[i].AbilityTypeID == abilityType)
                    return abilityConfigs[i];
            }
            throw new NullReferenceException();
        }
    }
}