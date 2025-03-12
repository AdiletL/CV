using System;
using System.Collections.Generic;
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
        private float duration, countDuration;
        private float addedStatValue;
        
        public SlowMovementEffect(SlowMovementConfig slowMovementConfig, string id) : base(slowMovementConfig, id)
        {
            this.value = slowMovementConfig.Value;
            this.duration = slowMovementConfig.Duration;
            this.valueType = slowMovementConfig.ValueType;
            this.IsInterim = slowMovementConfig.IsInterim;
        }

        public override void ClearValues()
        {
            addedStatValue = 0;
            countDuration = 0;
        }

        public override void Update()
        {
            if(!IsInterim) return;
            
            countDuration += Time.deltaTime;
            if (countDuration >= duration)
            {
                DestroyEffect();
                countDuration = 0;
            }
        }

        public override void FixedUpdate()
        {
            
        }

        public override void UpdateEffect()
        {
            countDuration = 0;
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
            movementStat.RemoveValue(addedStatValue);
        }

        public override void DestroyEffect()
        {
            var characterStatsController = Target.GetComponent<CharacterStatsController>();
            if(!characterStatsController) return;
            
            var movementStat = characterStatsController.GetStat(StatType.MovementSpeed);
            movementStat.AddValue(addedStatValue);
            ClearValues();
            base.DestroyEffect();
        }
    }

    [System.Serializable]
    public class SlowMovementConfig : EffectConfig
    {
        public ValueType ValueType;
        public float Value;
        public float Duration;
    }
}