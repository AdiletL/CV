using UnityEngine;

namespace Character
{
    public class CharacterMovementState : State, IMove
    {
        public GameObject GameObject;

        public float MovementSpeed { get; set; }
        public float RotationSpeed  { get; set; }
        
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
        public virtual void Rotate()
        {
            
        }
    }

    public class CharacterMovementStateBuilder : StateBuilder<CharacterMovementState>
    {
        public CharacterMovementStateBuilder(CharacterMovementState instance) : base(instance)
        {
        }

        public CharacterMovementStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }
        
        public CharacterMovementStateBuilder SetMovementSpeed(float speed)
        {
            state.MovementSpeed = speed;
            return this;
        }

        public CharacterMovementStateBuilder SetRotateSpeed(float speed)
        {
            state.RotationSpeed = speed;
            return this;
        }
    }
}