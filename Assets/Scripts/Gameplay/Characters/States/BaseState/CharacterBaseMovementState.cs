using UnityEngine;

namespace Character
{
    public class CharacterBaseMovementState : State, IMove
    {
        public override StateCategory Category { get; } = StateCategory.move;
        
        public GameObject GameObject { get; set; }
        
        public float MovementSpeed {get; set;}
        
        public override void Initialize()
        {
            
        }
        
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
        
    }
}