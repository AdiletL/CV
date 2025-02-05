using Machine;
using ScriptableObjects.Gameplay.Trap.Tower;
using Unity.Collections;
using UnityEngine;

namespace Unit.Trap.Tower
{
    public abstract class TowerController : TrapController, ITower, IClickableObject
    {
        [Space] [SerializeField] protected Transform pointSpawnProjectile;
        
        [field: SerializeField, Space] public GameObject SelectedObjectVisual { get; private set; }
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        protected SO_Tower so_Tower;
        protected StateMachine stateMachine;
        
        public override void Initialize()
        {
            base.Initialize();

            InitializeConfig();
            
            stateMachine = new StateMachine();
            
            CreateState();
            
            stateMachine.Initialize();
            stateMachine.OnChangedState += OnChangedState;
        }

        protected virtual void InitializeConfig()
        {
            so_Tower = (SO_Tower)so_Trap;
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
        
        private void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }

        private void OnDestroy()
        {
            stateMachine.OnChangedState -= OnChangedState;
        }

        public UnitInformation UnitInformation { get; }
        public void ShowInformation()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateInformation()
        {
            throw new System.NotImplementedException();
        }

        public void HideInformation()
        {
            throw new System.NotImplementedException();
        }
        
        public void SelectObject() => SelectedObjectVisual.SetActive(true);
        public void UnSelectObject() => SelectedObjectVisual.SetActive(false);
    }
}