using Calculate;
using Machine;
using UnityEngine;

namespace Unit
{
    public abstract class UnitIdleState : State
    {
        public override StateCategory Category { get; } = StateCategory.idle;
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        
        public override void Initialize()
        {
        }


        public override void Update()
        {
        }
        public override void LateUpdate()
        {
            
        }

    }

    public class UnitIdleStateBuilder : StateBuilder<UnitIdleState>
    {
        public UnitIdleStateBuilder(UnitIdleState instance) : base(instance)
        {
        }

        public UnitIdleStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }

        public UnitIdleStateBuilder SetCenter(Transform center)
        {
            state.Center = center;
            return this;
        }
    }
}