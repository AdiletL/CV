using System;
using Gameplay.Ability;
using ScriptableObjects.Ability;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class AbilityFactory : Factory
    {
        public Ability.Ability CreateAbility(AbilityConfig abilityConfig)
        {
            Ability.Ability result = abilityConfig.AbilityTypeID switch
            {
                _ when abilityConfig.AbilityTypeID == AbilityType.Nothing => null,
                _ when abilityConfig.AbilityTypeID == AbilityType.Dash => CreateDash(abilityConfig),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return result;
        }

        private DashAbility CreateDash(AbilityConfig abilityConfig)
        {
            return new DashAbility(abilityConfig as DashConfig);
        }
    }
}