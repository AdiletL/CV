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
        
        public override void Initialize()
        {
            base.Initialize();
            
            this.StateMachine.OnChangedState += OnChangedState;
        }
        

        public void Update()
        {
            this.StateMachine?.Update();
        }

        public void LateUpdate()
        {
            this.StateMachine?.LateUpdate();
        }
        
        protected void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.StateMachine.OnChangedState -= OnChangedState;
        }
    }
}