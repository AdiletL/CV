using System;
using UnityEngine;
using Zenject;

namespace ScriptableObjects.Ability
{
    [CreateAssetMenu(fileName = "SO_AbilityContainer", menuName = "SO/Gameplay/Ability/Container", order = 51)]
    public class SO_AbilityContainer : ScriptableObjectInstaller<SO_AbilityContainer>
    {
        [SerializeField] private SO_Ability[] abilityConfigs;

        public SO_Ability GetAbilityConfig(AbilityType abilityType)
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