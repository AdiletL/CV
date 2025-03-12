using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.Factory
{
    public class AbilityFactory : Factory
    {
        public Ability.Ability CreateAbility(AbilityConfig abilityConfig)
        {
            Ability.Ability result = abilityConfig.SO_BaseAbilityConfig.AbilityType switch
            {
                _ when abilityConfig.SO_BaseAbilityConfig.AbilityType == AbilityType.Dash => CreateDash(abilityConfig),
                _ when abilityConfig.SO_BaseAbilityConfig.AbilityType == AbilityType.Nothing => null,
            };
            
            return result;
        }

        private DashAbility CreateDash(AbilityConfig abilityConfig)
        {
            var dashConfig = abilityConfig as DashConfig;
            return (DashAbility)new DashAbilityBuilder()
                .SetDuration(dashConfig.Duration)
                .SetSpeed(dashConfig.Speed)
                .SetBlockedInputType(dashConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetAbilityBehaviour(dashConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetTimerCast(dashConfig.TimerCast)
                .SetCooldown(dashConfig.Cooldown)
                .Build();
        }
    }
}