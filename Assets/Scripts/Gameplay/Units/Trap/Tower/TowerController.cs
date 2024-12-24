using Machine;
using ScriptableObjects.Gameplay.Trap.Tower;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Trap.Tower
{
    public abstract class TowerController : TrapController, ITower
    {
        protected SO_Tower so_Tower;

        [Space] [SerializeField] protected Transform pointSpawnProjectile;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        protected StateMachine stateMachine;
        
        public override void Initialize()
        {
            base.Initialize();

            so_Tower = (SO_Tower)so_Trap;
            
            stateMachine = new StateMachine();
            
            CreateState();
            
            stateMachine.Initialize();
            stateMachine.OnChangedState += OnChangedState;
        }

        protected abstract void CreateState();
        
        private void Update()
        {
            this.stateMachine?.Update();
        }

        private void LateUpdate()
        {
            this.stateMachine?.LateUpdate();
        }
        
        private void OnChangedState(StateCategory category, Machine.IState state)
        {
            currentStateCategory = category;
            currentStateName = state.GetType().Name;
        }

        private void OnDestroy()
        {
            stateMachine.OnChangedState -= OnChangedState;
        }
    }
}