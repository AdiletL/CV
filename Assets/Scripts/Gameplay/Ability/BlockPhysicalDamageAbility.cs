using System;
using Gameplay.Resistance;
using UnityEngine;

namespace Gameplay.Ability
{
    public class BlockPhysicalDamageAbility : Ability
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

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if(!isActivated) return;
            StartEffect();
        }

        protected override void AfterCast()
        {
            base.AfterCast();
            resistanceHandler.AddResistance(normalDamageResistance);
        }

        public override void Exit()
        {
            if(!isActivated) return;
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
    
    public class BlockPhysicalDamageBuilder : AbilityBuilder<BlockPhysicalDamageAbility>
    {
        public BlockPhysicalDamageBuilder() : base(new BlockPhysicalDamageAbility())
        {
        }

        public BlockPhysicalDamageBuilder SetNormalDamageResistanceConfig(NormalDamageResistanceConfig config)
        {
            if (ability is BlockPhysicalDamageAbility blockPhysicalDamage)
                blockPhysicalDamage.SetNormalDamageResistance(config);
            return this;
        }
    }
}