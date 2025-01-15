using Machine;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterBaseMovementState : State, IMove
    {
        public override StateCategory Category { get; } = StateCategory.move;
        
        public GameObject GameObject { get; set; }
        
        public float MovementSpeed { get; set; }
        
        public override void Initialize()
        {
            
        }
        
        public override void Enter()
        {
        }

        public override void Update()
        {
        }

        public override void LateUpdate()
        {
            
        }

        public override void Exit()
        {
            
        }

        public abstract void ExecuteMovement();
        
        public virtual void IncreaseMovementSpeed(float value)
        {
            MovementSpeed += value;
        }

        public virtual void DecreaseMovementSpeed(float value)
        {
            MovementSpeed -= value;
        }
    }

    public class CharacterBaseMovementStateBuilder : StateBuilder<CharacterBaseMovementState>
    {
        public CharacterBaseMovementStateBuilder(CharacterBaseMovementState instance) : base(instance)
        {
        }

        public CharacterBaseMovementStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }
        public CharacterBaseMovementStateBuilder SetMovementSpeed(float speed)
        {
            state.MovementSpeed = speed;
            return this;
        }
        
    }
}