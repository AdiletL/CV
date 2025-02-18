using System;
using Gameplay.Resistance;
using UnityEngine;

namespace Gameplay.Ability
{
    public class BlockPhysicalDamage : Ability
    {
        public override AbilityType AbilityType { get; protected set; } = AbilityType.BlockPhysicalDamage;

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

        public override void Activate(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Activate(finishedCallBack, target, point);
            if(!isActivated) return;
            resistanceHandler.AddResistance(normalDamageResistance);
        }

        public override void Exit()
        {
            resistanceHandler.RemoveResistance(normalDamageResistance);
            base.Exit();
        }
    }

    [System.Serializable]
    public class BlockPhysicalDamageConfig : AbilityConfig
    {
        public NormalDamageResistanceConfig NormalDamageResistanceConfig;
        public AnimationClip BlockClip;
    }
    
    public class BlockPhysicalDamageBuilder : AbilityBuilder<BlockPhysicalDamage>
    {
        public BlockPhysicalDamageBuilder() : base(new BlockPhysicalDamage())
        {
        }

        public BlockPhysicalDamageBuilder SetNormalDamageResistanceConfig(NormalDamageResistanceConfig config)
        {
            if (ability is BlockPhysicalDamage blockPhysicalDamage)
                blockPhysicalDamage.SetNormalDamageResistance(config);
            return this;
        }
    }
}