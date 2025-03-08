using Machine;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public abstract class TrapMovementState : State, IMovement
    {
        public override StateCategory Category { get; } = StateCategory.Move;

        public GameObject GameObject { get; set; }
        public Stat MovementSpeedStat { get; }
        
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