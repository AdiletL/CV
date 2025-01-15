using Machine;
using UnityEngine;

namespace Unit.Trap
{
    public abstract class TrapMovementState : State, IMove
    {
        public override StateCategory Category { get; } = StateCategory.move;

        public GameObject GameObject { get; set; }
        
        public float MovementSpeed { get; }

        public abstract void ExecuteMovement();
    }

    public class TrapMovementStateBuilder : StateBuilder<TrapMovementState>
    {
        public TrapMovementStateBuilder(TrapMovementState instance) : base(instance)
        {
        }

        public TrapMovementStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }
    }
}