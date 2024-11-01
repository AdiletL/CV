using UnityEngine;

namespace Character
{
    public class CharacterBaseMovementState : State, IMove
    {
        public GameObject GameObject;
        
        public float MovementSpeed {get; set;}
        public float RotationSpeed { get; set; }
        
        public override void Enter()
        {
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            
        }

        public virtual void Move()
        {
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

        public CharacterBaseMovementStateBuilder SetRotateSpeed(float speed)
        {
            state.RotationSpeed = speed;
            return this;
        }
    }
}