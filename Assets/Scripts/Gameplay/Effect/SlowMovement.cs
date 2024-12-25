using System.Collections.Generic;
using Unit.Character;
using UnityEngine;

namespace Gameplay.Effect
{
    public class SlowMovement : Effect
    {
        private List<CharacterBaseMovementState> movementStates = new ();
        private float value;
        private float duration, countDuration;
        
        public SlowMovement(float value, float duration) : base()
        {
            this.value = value;
            this.duration = duration;
        }

        public override void ResetEffect()
        {
            countDuration = 0;
            movementStates.Clear();
        }

        public override void UpdateEffect()
        {
            countDuration += Time.deltaTime;

            if (countDuration >= duration)
            {
                DestroyEffect();
                countDuration = 0;
            }
        }

        public override void ApplyEffect()
        {
            var character = target.GetComponent<CharacterMainController>();
            if(character == null) return;
            
            var movementStates = character.GetStates<CharacterBaseMovementState>();

            foreach (var VARIABLE in movementStates)
            {
                Debug.Log(VARIABLE.GetType());
                VARIABLE.DecreaseMovementSpeed(value);
                this.movementStates.Add(VARIABLE);
            }
        }

        public override void DestroyEffect()
        {
            foreach (var VARIABLE in movementStates)
            {
                VARIABLE.IncreaseMovementSpeed(value);
            }
            ResetEffect();
        }
    }
}