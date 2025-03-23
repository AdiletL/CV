using System;
using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.Factory
{
    public class AbilityFactory : Factory
    {
        public Ability.Ability CreateAbility(AbilityConfig abilityConfig)
        {
            Ability.Ability result = abilityConfig.SO_BaseAbilityConfig.AbilityTypeID switch
            {
                _ when abilityConfig.SO_BaseAbilityConfig.AbilityTypeID == AbilityType.Nothing => null,
                _ when abilityConfig.SO_BaseAbilityConfig.AbilityTypeID == AbilityType.Dash => CreateDash(abilityConfig),
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