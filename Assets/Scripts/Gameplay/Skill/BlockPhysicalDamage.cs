using System;
using Gameplay.Resistance;
using UnityEngine;

namespace Gameplay.Skill
{
    public class BlockPhysicalDamage : Skill
    {
        public override SkillType SkillType { get; protected set; } = SkillType.blockPhysicalDamage;

        private ResistanceHandler resistanceHandler;
        private NormalDamageResistance normalDamageResistance;


        public void SetNormalDamageResistance(NormalDamageResistanceConfig config)
        {
            normalDamageResistance = new NormalDamageResistance(config.Value, config.ValueType);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            resistanceHandler = GameObject.GetComponent<ResistanceHandler>();
        }

        public override void Execute(Action exitCallBack = null)
        {
            base.Execute(exitCallBack);
            resistanceHandler.AddResistance(normalDamageResistance);
        }

        public override void Exit()
        {
            resistanceHandler.RemoveResistance(normalDamageResistance);
            base.Exit();
        }
    }

    [System.Serializable]
    public class BlockPhysicalDamageConfig : SkillConfig
    {
        public NormalDamageResistanceConfig NormalDamageResistanceConfig;
        public AnimationClip BlockClip;
    }
    
    public class BlockPhysicalDamageBuilder : SkillBuilder<BlockPhysicalDamage>
    {
        public BlockPhysicalDamageBuilder() : base(new BlockPhysicalDamage())
        {
        }

        public BlockPhysicalDamageBuilder SetNormalDamageResistanceConfig(NormalDamageResistanceConfig config)
        {
            if (skill is BlockPhysicalDamage blockPhysicalDamage)
                blockPhysicalDamage.SetNormalDamageResistance(config);
            return this;
        }
    }
}