using Gameplay.Factory;
using Gameplay.Factory.Character.Creep;
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
        protected CreepSwitchStateFactory creepSwitchStateFactory;
        protected CharacterAnimation characterAnimation;
        protected CharacterEndurance characterEndurance;
        protected CharacterExperience characterExperience;
        protected Gravity gravity;

        protected abstract CreepStateFactory CreateCreepStateFactory();
        protected abstract CreepSwitchStateFactory CreateCreepSwitchStateFactory();


        public override void Initialize()
        {
            base.Initialize();
            
            characterExperience = GetComponentInUnit<CharacterExperience>();
            diContainer.Inject(characterExperience);
            characterExperience.Initialize();
            
            characterEndurance = GetComponentInUnit<CharacterEndurance>();
            diContainer.Inject(characterEndurance);
            characterEndurance.Initialize();
            
            gravity = GetComponentInUnit<Gravity>();
            gravity.InActivateGravity();
        }

        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();
            navMeshAgent = GetComponentInUnit<NavMeshAgent>();
            characterAnimation = GetComponentInUnit<CharacterAnimation>();
            characterAnimation.Initialize();
            
            creepStateFactory = CreateCreepStateFactory();
            creepStateFactory.Initialize();
            creepSwitchStateFactory = CreateCreepSwitchStateFactory();
            creepSwitchStateFactory.Initialize();
        }

        public override void Appear()
        {
            navMeshAgent.enabled = true;
            gravity.ActivateGravity();
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