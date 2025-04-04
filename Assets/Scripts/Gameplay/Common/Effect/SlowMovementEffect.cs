using System;
using Calculate;
using Gameplay.Unit;
using Gameplay.Unit.Character;
using UnityEngine;
using ValueType = Calculate.ValueType;

namespace Gameplay.Effect
{
    public class SlowMovementEffect : Effect
    {
        public override EffectType EffectTypeID { get; } = EffectType.SlowMovement;

        private ValueType valueType;
        private float value;
        private float timer, countTimer;
        private float addedStatValue;
        
        public SlowMovementEffect(SlowMovementConfig slowMovementConfig) : base(slowMovementConfig)
        {
            this.value = slowMovementConfig.GameValueConfig.Value;
            this.timer = slowMovementConfig.Timer;
            this.valueType = slowMovementConfig.GameValueConfig.ValueTypeID;
        }

        public override void ClearValues()
        {
            addedStatValue = 0;
            countTimer = timer;
        }

        public override void Update()
        {
            if(!EffectCategoryID.HasFlag(EffectCategory.Interim)) return;
            
            countTimer -= Time.deltaTime;
            if (countTimer <= 0)
                DestroyEffect();
        }

        public override void FixedUpdate()
        {
            
        }

        public override void UpdateEffect()
        {
            countTimer = timer;
        }

        public override void ApplyEffect()
        {
            var characterStatsController = Target.GetComponent<UnitStatsController>();
            if(!characterStatsController)
            {
                DestroyEffect();
                return;
            }
            
            var movementStat = characterStatsController.GetStat(StatType.MovementSpeed);
            var gameValue = new GameValue(value, valueType);
            addedStatValue = gameValue.Calculate(movementStat.CurrentValue);
            movementStat.RemoveCurrentValue(addedStatValue);
        }

        public override void DestroyEffect()
        {
            var characterStatsController = Target.GetComponent<CharacterStatsController>();
            if(!characterStatsController) return;
            
            var movementStat = characterStatsController.GetStat(StatType.MovementSpeed);
            movementStat.AddCurrentValue(addedStatValue);
            ClearValues();
            base.DestroyEffect();
        }
    }

    [System.Serializable]
    public class SlowMovementConfig : EffectConfig
    {
        public GameValueConfig GameValueConfig;
        public float Timer;
    }
}