using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Character.Creep
{
    public abstract class CreepController : CharacterMainController
    {
        public override UnitType UnitType { get; } = UnitType.creep;

        [FormerlySerializedAs("so_EnemyMove")] [SerializeField] protected SO_CreepMove soCreepMove;
        [SerializeField] protected Transform center;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected Transform start;
        protected Transform end;
        
        public override void Initialize()
        {
            base.Initialize();

            CreateStates();
            
            this.StateMachine.Initialize();
            components.GetComponentFromArray<CreepHealth>()?.Initialize();
            
            this.StateMachine.SetStates(typeof(CreepIdleState));
            
            this.StateMachine.OnChangedState += OnChangedState;
        }

        protected abstract void CreateStates();
        
                
        public void SetStart(Transform start) => this.start = start;
        public void SetEnd(Transform end) => this.end = end;

        public void Update()
        {
            this.StateMachine?.Update();
        }

        public void LateUpdate()
        {
            this.StateMachine?.LateUpdate();
        }
        
        private void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }
        
        private void OnDestroy()
        {
            this.StateMachine.OnChangedState -= OnChangedState;
        }
    }
}