using System;
using Gameplay.Skill;
using ScriptableObjects.Gameplay.Skill;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class SkillFactory : Factory
    {
        [Inject] private SO_SkillContainer so_SkillContainer;
        
        private GameObject gameObject;
        private IMoveControl moveControl;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetMoveControl(IMoveControl moveControl) => this.moveControl = moveControl;
        
        
        public Skill.Skill CreateSkill(SkillType skillType)
        {
            Skill.Skill result = skillType switch
            {
                _ when skillType == SkillType.dash => CreateDash(),
                _ when skillType == SkillType.spawnTeleport => CreateSpawnPortal(),
                _ when skillType == SkillType.nothing => null,
            };
            
            return result;
        }

        private Dash CreateDash()
        {
            var dashConfig = so_SkillContainer.GetSkillConfig<SO_SkillDash>();
            if (dashConfig == null)
                throw new NullReferenceException();
            
            return (Dash)new DashBuilder()
                .SetMoveControl(moveControl)
                .SetDuration(dashConfig.DashDuration)
                .SetSpeed(dashConfig.DashSpeed)
                .SetBlockedInputType(dashConfig.BlockedInputType)
                .SetBlockedSkillType(dashConfig.BlockedSkillType)
                .SetGameObject(gameObject)
                .Build();
        }

        private SpawnPortal CreateSpawnPortal()
        {
            var spawnPortalConfig = so_SkillContainer.GetSkillConfig<SO_SkillSpawnPortal>();
            if (spawnPortalConfig == null)
                throw new NullReferenceException();

            return (SpawnPortal)new SpawnPortalBuilder()
                .SetPortalObject(spawnPortalConfig.SpawnPortalPrefab)
                .SetIDStartPortal(spawnPortalConfig.IDStartPortal.ID)
                .SetGameObject(gameObject)
                .SetBlockedInputType(spawnPortalConfig.BlockedInputType)
                .SetBlockedSkillType(spawnPortalConfig.BlockedSkillType)
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