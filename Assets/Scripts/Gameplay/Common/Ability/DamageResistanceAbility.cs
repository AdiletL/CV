using System;
using System.Collections.Generic;
using Calculate;
using Gameplay.Resistance;
using Gameplay.Unit;
using ScriptableObjects.Ability;
using UnityEngine;
using ValueType = Calculate.ValueType;

namespace Gameplay.Ability
{
    public class DamageResistanceAbility : Ability
    {
        public override AbilityType AbilityTypeID { get; protected set; } = AbilityType.DamageResistance;

        private SO_DamageResistanceAbility so_DamageResistanceAbility;
        private UnitStatsController unitStatsController;
        private UnitStatConfig[] statConfigs;

        private bool isUsed;
        
        private List<float> addedStatValues = new List<float>();


        public DamageResistanceAbility(SO_DamageResistanceAbility so_DamageResistanceAbility) : base(so_DamageResistanceAbility)
        {
            this.so_DamageResistanceAbility = so_DamageResistanceAbility;
        }

        public override void Initialize()
        {
            base.Initialize();
            unitStatsController = GameObject.GetComponent<UnitStatsController>();
            statConfigs = so_DamageResistanceAbility.StatConfigs;
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
                stat = unitStatsController.GetStat(statConfig.UnitStatTypeID);
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
                stat = unitStatsController.GetStat(config.UnitStatTypeID);
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
}