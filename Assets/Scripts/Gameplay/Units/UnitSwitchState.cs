using Machine;
using UnityEngine;

namespace Unit
{
    public abstract class UnitSwitchState : ISwitchState
    {
        protected GameObject gameObject;
        protected StateMachine stateMachine;
        protected Transform center;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCenter(Transform center) => this.center = center;
        
        public abstract void Initialize();
        public abstract void SetState();
        public abstract void ExitOtherStates();
        public abstract void ExitCategory(StateCategory category);
    }

    public abstract class UnitSwitchStateBuilder<T> where T : UnitSwitchState
    {
        protected UnitSwitchState switchState;
        
        public UnitSwitchStateBuilder(UnitSwitchState instance)
        {
            switchState = instance;
        }

        public UnitSwitchStateBuilder<T> SetGameObject(GameObject gameObject)
        {
            switchState.SetGameObject(gameObject);
            return this;
        }
        
        public UnitSwitchStateBuilder<T> SetCenter(Transform center)
        {
            switchState.SetCenter(center);
            return this;
        }
        
        public UnitSwitchStateBuilder<T> SetStateMachine(StateMachine stateMachine)
        {
            switchState.SetStateMachine(stateMachine);
            return this;
        }
        
        public UnitSwitchState Build()
        {
            return switchState;
        }
    }
}