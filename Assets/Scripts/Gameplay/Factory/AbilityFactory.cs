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
            Ability.Ability result = abilityConfig.AbilityType switch
            {
                _ when abilityConfig.AbilityType == AbilityType.Dash => CreateDash(abilityConfig),
                _ when abilityConfig.AbilityType == AbilityType.SpawnPortal => CreateSpawnPortal(abilityConfig),
                _ when abilityConfig.AbilityType == AbilityType.ApplyDamageHeal => CreateApplyDamageHeal(abilityConfig),
                _ when abilityConfig.AbilityType == AbilityType.Nothing => null,
            };
            
            return result;
        }

        private Dash CreateDash(AbilityConfig abilityConfig)
        {
            var dashConfig = abilityConfig as DashConfig;
            return (Dash)new DashBuilder()
                .SetMoveControl(moveControl)
                .SetDuration(dashConfig.Duration)
                .SetSpeed(dashConfig.Speed)
                .SetBlockedInputType(dashConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(dashConfig.AbilityBehaviour)
                .SetCooldown(dashConfig.Cooldown)
                .Build();
        }

        private SpawnPortal CreateSpawnPortal(AbilityConfig abilityConfig)
        {
            var spawnPortalConfig = abilityConfig as SpawnPortalConfig;
            return (SpawnPortal)new SpawnPortalBuilder()
                .SetPortalObject(spawnPortalConfig.SpawnPortalPrefab)
                .SetIDStartPortal(spawnPortalConfig.StartPortalID.ID)
                .SetBaseCamera(baseCamera)
                .SetGameObject(gameObject)
                .SetBlockedInputType(spawnPortalConfig.BlockedInputType)
                .SetAbilityBehaviour(spawnPortalConfig.AbilityBehaviour)
                .SetCooldown(spawnPortalConfig.Cooldown)
                .Build();
        }

        private ApplyDamageHeal CreateApplyDamageHeal(AbilityConfig abilityConfig)
        {
            var applyDamageHealConfig = abilityConfig as ApplyDamageHealConfig;
            return (ApplyDamageHeal)new ApplyDamageHealBuilder()
                .SetValueType(applyDamageHealConfig.ValueType)
                .SetValue(applyDamageHealConfig.Value)
                .SetBlockedInputType(applyDamageHealConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(applyDamageHealConfig.AbilityBehaviour)
                .SetCooldown(applyDamageHealConfig.Cooldown)
                .Build();
        }
    }

    public class SkillFactoryBuilder
    {
        private AbilityFactory _abilityFactory;

        public SkillFactoryBuilder(AbilityFactory abilityFactory)
        {
            this._abilityFactory = abilityFactory;
        }

        public SkillFactoryBuilder SetGameObject(GameObject gameObject)
        {
            _abilityFactory.SetGameObject(gameObject);
            return this;
        }

        public SkillFactoryBuilder SetMoveControl(IMoveControl moveControl)
        {
            _abilityFactory.SetMoveControl(moveControl);
            return this;
        }
        
        public SkillFactoryBuilder SetBaseCamera(Camera camera)
        {
            _abilityFactory.SetBaseCamera(camera);
            return this;
        }

        public AbilityFactory Build()
        {
            return _abilityFactory;
        }
    }
}