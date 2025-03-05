using Machine;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterBaseMovementState : State, IMovement
    {
        public override StateCategory Category { get; } = StateCategory.Move;

        protected GameObject gameObject;
        protected Transform center;
        
        public Stat MovementSpeedStat { get; protected set; } = new Stat();

        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        
        
        public override void Update()
        {
        }

        public override void LateUpdate()
        {
            
        }
        
        public abstract void ExecuteMovement();
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
        public CharacterBaseMovementStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
    }
}