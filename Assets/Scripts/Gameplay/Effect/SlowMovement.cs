using System;
using System.Collections.Generic;
using Calculate;
using Unit.Character;
using UnityEngine;
using ValueType = Calculate.ValueType;

namespace Gameplay.Effect
{
    public class SlowMovement : Effect
    {
        private struct MovementInfo
        {
            public CharacterBaseMovementState MovementState { get; }
            public float Speed { get; }

            public MovementInfo(CharacterBaseMovementState movementState, float speed)
            {
                MovementState = movementState;
                Speed = speed;
            }
        }

        
        private List<MovementInfo> movementStates = new ();
        private float value;
        private float duration, countDuration;
        private ValueType valueType;
        
        public SlowMovement(SlowMovementInfo slowMovementInfo) : base()
        {
            this.value = slowMovementInfo.Value;
            this.duration = slowMovementInfo.Duration;
            this.valueType = slowMovementInfo.ValueType;
        }

        public override void ClearValues()
        {
            countDuration = 0;
            movementStates.Clear();
        }

        public override void Update()
        {
            countDuration += Time.deltaTime;

            if (countDuration >= duration)
            {
                DestroyEffect();
                countDuration = 0;
            }
        }

        public override void LateUpdate()
        {
            
        }

        public override void ApplyEffect()
        {
            var character = target.GetComponent<CharacterMainController>();
            if(character == null) return;
            
            var movementStates = character.GetStates<CharacterBaseMovementState>();
            var gameValue = new GameValue(value, valueType);
            var result = 0;
            
            foreach (var VARIABLE in movementStates)
            {
                result = gameValue.Calculate(VARIABLE.CurrentMovementSpeed);
                VARIABLE.RemoveMovementSpeed(result);
                var movementInfo = new MovementInfo(VARIABLE, result);
                this.movementStates.Add(movementInfo);
            }
        }

        public override void DestroyEffect()
        {
            base.DestroyEffect();
            foreach (var VARIABLE in movementStates)
            {
                VARIABLE.MovementState.AddMovementSpeed(VARIABLE.Speed);
            }
            ClearValues();
        }
    }

    [System.Serializable]
    public class SlowMovementInfo
    {
        public int Value;
        public float Duration;
        public ValueType ValueType;
    }
}