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
                _ when abilityConfig.SO_BaseAbilityConfig.AbilityType == AbilityType.Vampirism => CreateApplyDamageHeal(abilityConfig),
                _ when abilityConfig.SO_BaseAbilityConfig.AbilityType == AbilityType.Nothing => null,
            };
            
            return result;
        }

        private DashAbility CreateDash(AbilityConfig abilityConfig)
        {
            var dashConfig = abilityConfig as DashConfig;
            return (DashAbility)new DashBuilder()
                .SetDuration(dashConfig.Duration)
                .SetSpeed(dashConfig.Speed)
                .SetBlockedInputType(dashConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetGameObject(owner)
                .SetAbilityBehaviour(dashConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetTimerCast(dashConfig.TimerCast)
                .SetCooldown(dashConfig.Cooldown)
                .Build();
        }

        private VampirismAbility CreateApplyDamageHeal(AbilityConfig abilityConfig)
        {
            var applyDamageHealConfig = abilityConfig as VampirismConfig;
            return (VampirismAbility)new ApplyDamageHealBuilder()
                .SetOwner(owner)
                .SetValueType(applyDamageHealConfig.ValueType)
                .SetValue(applyDamageHealConfig.Value)
                .SetBlockedInputType(applyDamageHealConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetGameObject(owner)
                .SetAbilityBehaviour(applyDamageHealConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetTimerCast(applyDamageHealConfig.TimerCast)
                .SetCooldown(applyDamageHealConfig.Cooldown)
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