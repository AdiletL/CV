using System.Collections.Generic;
using Gameplay.Skill;
using UnityEngine;

namespace Gameplay.Factory
{
    public class SkillFactory : Factory
    {
        private GameObject gameObject;
        private IMoveControl moveControl;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;
        
        
        public Skill.Skill CreateSkill(SkillConfig skillConfig)
        {
            Skill.Skill result = skillConfig.SkillType switch
            {
                _ when skillConfig.SkillType == SkillType.dash => CreateDash(skillConfig),
                _ when skillConfig.SkillType == SkillType.spawnTeleport => CreateSpawnPortal(skillConfig),
                _ when skillConfig.SkillType == SkillType.applyDamageHeal => CreateApplyDamageHeal(skillConfig),
                _ when skillConfig.SkillType == SkillType.nothing => null,
            };
            
            return result;
        }

        private Dash CreateDash(SkillConfig skillConfig)
        {
            var dashConfig = skillConfig as DashConfig;
            return (Dash)new DashBuilder()
                .SetMoveControl(moveControl)
                .SetDuration(dashConfig.Duration)
                .SetSpeed(dashConfig.Speed)
                .SetBlockedInputType(dashConfig.BlockedInputType)
                .SetBlockedSkillType(dashConfig.BlockedSkillType)
                .SetGameObject(gameObject)
                .Build();
        }

        private SpawnPortal CreateSpawnPortal(SkillConfig skillConfig)
        {
            var spawnPortalConfig = skillConfig as SpawnPortalConfig;
            return (SpawnPortal)new SpawnPortalBuilder()
                .SetPortalObject(spawnPortalConfig.SpawnPortalPrefab)
                .SetIDStartPortal(spawnPortalConfig.StartPortalID.ID)
                .SetGameObject(gameObject)
                .SetBlockedInputType(spawnPortalConfig.BlockedInputType)
                .SetBlockedSkillType(spawnPortalConfig.BlockedSkillType)
                .Build();
        }

        private ApplyDamageHeal CreateApplyDamageHeal(SkillConfig skillConfig)
        {
            var applyDamageHealConfig = skillConfig as ApplyDamageHealConfig;
            return (ApplyDamageHeal)new ApplyDamageHealBuilder()
                .SetValueType(applyDamageHealConfig.ValueType)
                .SetValue(applyDamageHealConfig.Value)
                .SetBlockedSkillType(applyDamageHealConfig.BlockedSkillType)
                .SetBlockedInputType(applyDamageHealConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .Build();
        }
    }

    public class SkillFactoryBuilder
    {
        private SkillFactory skillFactory;

        public SkillFactoryBuilder(SkillFactory skillFactory)
        {
            this.skillFactory = skillFactory;
        }

        public SkillFactoryBuilder SetGameObject(GameObject gameObject)
        {
            skillFactory.SetGameObject(gameObject);
            return this;
        }

        public SkillFactoryBuilder SetMoveControl(IMoveControl moveControl)
        {
            skillFactory.SetMoveControl(moveControl);
            return this;
        }

        public SkillFactory Build()
        {
            return skillFactory;
        }
    }
}