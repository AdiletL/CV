using Machine;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterBaseMovementState : State, IMovement
    {
        public override StateCategory Category { get; } = StateCategory.Move;

        protected GameObject gameObject;
        protected Transform center;
        
        public float BaseMovementSpeed { get; protected set; }
        public float CurrentMovementSpeed { get; protected set; }
        
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        public void SetBaseMovementSpeed(float movementSpeed) => this.BaseMovementSpeed = movementSpeed;
        
        
        public override void Initialize()
        {
            CurrentMovementSpeed = BaseMovementSpeed;
        }
        
        public override void Update()
        {
        }

        public override void LateUpdate()
        {
            
        }
        
        public abstract void ExecuteMovement();
        
        public virtual void AddMovementSpeed(float value)
        {
            CurrentMovementSpeed += value;
        }

        public virtual void RemoveMovementSpeed(float value)
        {
            CurrentMovementSpeed -= value;
        }
    }

    public class CharacterBaseMovementStateBuilder : StateBuilder<CharacterBaseMovementState>
    {
        public CharacterBaseMovementStateBuilder(CharacterBaseMovementState instance) : base(instance)
        {
        }

        public CharacterBaseMovementStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }
        public CharacterBaseMovementStateBuilder SetMovementSpeed(float speed)
        {
            state.SetBaseMovementSpeed(speed);
            return this;
        }
        public CharacterBaseMovementStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
    }
}