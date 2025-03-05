using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.Factory
{
    public class AbilityFactory : Factory
    {
        private GameObject gameObject;
        private IMoveControl moveControl;
        private Camera baseCamera;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetBaseCamera(Camera camera) => this.baseCamera = camera;
        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;
        
        
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
                .SetMoveControl(moveControl)
                .SetDuration(dashConfig.Duration)
                .SetSpeed(dashConfig.Speed)
                .SetBlockedInputType(dashConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(dashConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetTimerCast(dashConfig.TimerCast)
                .SetCooldown(dashConfig.Cooldown)
                .Build();
        }

        private VampirismAbility CreateApplyDamageHeal(AbilityConfig abilityConfig)
        {
            var applyDamageHealConfig = abilityConfig as VampirismConfig;
            return (VampirismAbility)new ApplyDamageHealBuilder()
                .SetOwner(gameObject)
                .SetValueType(applyDamageHealConfig.ValueType)
                .SetValue(applyDamageHealConfig.Value)
                .SetBlockedInputType(applyDamageHealConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(applyDamageHealConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetTimerCast(applyDamageHealConfig.TimerCast)
                .SetCooldown(applyDamageHealConfig.Cooldown)
                .Build();
        }
    }

    public class AbilityFactoryBuilder
    {
        private AbilityFactory abilityFactory = new ();

        public AbilityFactoryBuilder SetGameObject(GameObject gameObject)
        {
            abilityFactory.SetGameObject(gameObject);
            return this;
        }

        public AbilityFactoryBuilder SetMoveControl(IMoveControl moveControl)
        {
            abilityFactory.SetMoveControl(moveControl);
            return this;
        }
        
        public AbilityFactoryBuilder SetBaseCamera(Camera camera)
        {
            abilityFactory.SetBaseCamera(camera);
            return this;
        }

        public AbilityFactory Build()
        {
            return abilityFactory;
        }
    }
}