using System;
using System.Collections.Generic;
using Calculate;
using Gameplay.Resistance;
using Gameplay.Unit;
using UnityEngine;
using ValueType = Calculate.ValueType;

namespace Gameplay.Ability
{
    public class DamageResistanceAbility : Ability
    {
        public override AbilityType AbilityTypeID { get; protected set; } = AbilityType.DamageResistance;

        private DamageResistanceConfig config;
        private UnitStatsController unitStatsController;
        private StatConfig[] statConfigs;

        private bool isUsed;
        
        private List<float> addedStatValues = new List<float>();


        public DamageResistanceAbility(DamageResistanceConfig config) : base(config)
        {
            this.config = config;
        }

        public override void Initialize()
        {
            base.Initialize();
            unitStatsController = GameObject.GetComponent<UnitStatsController>();
            statConfigs = config.StatConfigs;
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
            AddStats();
            isUsed = true;
        }

        private void AddStats()
        {
            addedStatValues.Clear();
            Stat stat = null;
            float result = 0;
            foreach (var statConfig in statConfigs)
            {
                stat = unitStatsController.GetStat(statConfig.StatTypeID);
                foreach (var statValue in statConfig.StatValuesConfig)
                {
                    var gameValue = new GameValue(statValue.GameValueConfig.Value, statValue.GameValueConfig.ValueTypeID);
                    result = gameValue.Calculate(stat.CurrentValue);
                    stat.AddValue(result, statValue.StatValueTypeID);
                    addedStatValues.Add(result);
                }
            }
        }

        private void RemoveStats()
        {
            Stat stat = null;
            int index = 0;
            foreach (var config in statConfigs)
            {
                stat = unitStatsController.GetStat(config.StatTypeID);
                foreach (var statValue in config.StatValuesConfig)
                {
                    stat.RemoveValue(addedStatValues[index], statValue.StatValueTypeID);
                    index++;
                }
            }
        }

        public override void Exit()
        {
            if(!isActivated || !isUsed) return;
            RemoveStats();
            isUsed = false;
            base.Exit();
        }
    }

    [System.Serializable]
    public class DamageResistanceConfig : AbilityConfig
    {
        public override AbilityType AbilityTypeID { get; } = AbilityType.DamageResistance;
        public StatConfig[] StatConfigs;
        public AnimationClip Clip;
    }
}