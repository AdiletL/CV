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

        private UnitStatsController _unitStatsController;
        private StatConfig[] statConfigs;

        private bool isUsed;
        
        private List<float> addedStatValues = new List<float>();

        public void SetStatConfigs(StatConfig[] statConfigs) => this.statConfigs = statConfigs;

        public override void Initialize()
        {
            base.Initialize();
            _unitStatsController = GameObject.GetComponent<UnitStatsController>();
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
            for (int i = 0; i < statConfigs.Length; i++)
            {
                stat = _unitStatsController.GetStat(statConfigs[i].StatTypeID);
                for (int j = 0; j < statConfigs[i].StatValuesConfig.Length; j++)
                {
                    var statValue = statConfigs[i].StatValuesConfig[j];
                    var gameValue = new GameValue(statValue.Value, statValue.ValueTypeID);
                    
                    switch (statValue.StatValueTypeID)
                    {
                        case StatValueType.Current: 
                            result = gameValue.Calculate(stat.CurrentValue);
                            stat.AddCurrentValue(result);
                            break;
                        case StatValueType.Maximum:
                            result = gameValue.Calculate(stat.MaximumValue);
                            stat.AddMaxValue(result);
                            break;
                        case StatValueType.Minimum:
                            result = gameValue.Calculate(stat.MinimumValue);
                            stat.AddMinValue(result);
                            break;
                    }
                    
                    addedStatValues.Add(result);
                }
            }
        }

        private void RemoveStats()
        {
            Stat stat = null;
            for (int i = 0; i < statConfigs.Length; i++)
            {
                stat = _unitStatsController.GetStat(statConfigs[i].StatTypeID);
                for (int j = 0; j < statConfigs[i].StatValuesConfig.Length; j++)
                {
                    var statValue = statConfigs[i].StatValuesConfig[j];
                    switch (statValue.StatValueTypeID)
                    {
                        case StatValueType.Current: stat.RemoveCurrentValue(addedStatValues[i]); break;
                        case StatValueType.Maximum: stat.RemoveMaxValue(addedStatValues[i]); break;
                        case StatValueType.Minimum: stat.RemoveMinValue(addedStatValues[i]); break;
                    }
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
        public StatConfig[] StatConfigs;
        public AnimationClip Clip;
    }
    
    public class DamageResistanceAbilityBuilder : AbilityBuilder
    {
        public DamageResistanceAbilityBuilder() : base(new DamageResistanceAbility())
        {
        }

        public DamageResistanceAbilityBuilder SetStatConfigs(StatConfig[] configs)
        {
            if (ability is DamageResistanceAbility blockPhysicalDamage)
                blockPhysicalDamage.SetStatConfigs(configs);
            return this;
        }
    }
}