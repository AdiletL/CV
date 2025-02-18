using Calculate;
using Machine;
using UnityEngine;

namespace Unit
{
    public abstract class UnitIdleState : State
    {
        public override StateCategory Category { get; } = StateCategory.Idle;
        
        protected GameObject gameObject;
        protected Transform center;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        
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
            state.SetGameObject(gameObject);
            return this;
        }

        public UnitIdleStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
    }
}