using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.Factory
{
    public class AbilityFactory : Factory
    {
        private GameObject owner;
        
        public void SetOwner(GameObject gameObject) => this.owner = gameObject;
        
        
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
                .SetGameObject(owner)
                .SetAbilityBehaviour(dashConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetTimerCast(dashConfig.TimerCast)
                .SetCooldown(dashConfig.Cooldown)
                .Build();
        }
    }

    public class AbilityFactoryBuilder
    {
        private AbilityFactory abilityFactory = new ();

        public AbilityFactoryBuilder SetOwner(GameObject owner)
        {
            abilityFactory.SetOwner(owner);
            return this;
        }
        public AbilityFactory Build()
        {
            return abilityFactory;
        }
    }
}