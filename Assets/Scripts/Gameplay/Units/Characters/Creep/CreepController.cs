using Gameplay.Factory;
using Machine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class CreepController : CharacterMainController
    {
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected NavMeshAgent navMeshAgent;
        protected CreepStateFactory creepStateFactory;
        protected CharacterAnimation characterAnimation;

        protected abstract CreepStateFactory CreateCreepStateFactory();

        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();
            navMeshAgent = GetComponentInUnit<NavMeshAgent>();
            characterAnimation = GetComponentInUnit<CharacterAnimation>();
            characterAnimation.Initialize();
            creepStateFactory = CreateCreepStateFactory();
            creepStateFactory.Initialize();
        }

        public override void Appear()
        {
            navMeshAgent.enabled = true;
        }

        protected override void InitializeMediator()
        {
            base.InitializeMediator();
            this.StateMachine.OnChangedState += OnChangedState;
        }

        protected override void UnInitializeMediatorRPC()
        {
            base.UnInitializeMediatorRPC();
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