using Machine;
using Unity.Collections;
using UnityEngine;

namespace Unit.Character.Creep
{
    public abstract class CreepController : CharacterMainController
    {
        [SerializeField] protected Transform center;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        

        protected override void InitializeMediator()
        {
            base.InitializeMediator();
            this.StateMachine.OnChangedState += OnChangedState;
        }

        protected override void UnInitializeMediator()
        {
            base.UnInitializeMediator();
            this.StateMachine.OnChangedState -= OnChangedState;
        }

        protected void Update()
        {
            this.StateMachine?.Update();
        }

        protected void LateUpdate()
        {
            this.StateMachine?.LateUpdate();
        }
        
        protected void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }
    }
}