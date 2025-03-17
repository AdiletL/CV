using System;
using Gameplay.Ability;
using Gameplay.Effect;
using Gameplay.Factory.Character.Creep;
using Gameplay.Resistance;
using Machine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Unit.Character.Creep
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ResistanceHandler))]
    [RequireComponent(typeof(AbilityHandler))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CreepGravity))]
    [RequireComponent(typeof(EffectHandler))]
    public abstract class CreepController : CharacterMainController
    {
        public event Action<CreepController> OnDeath; 
        
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected NavMeshAgent navMeshAgent;
        protected CharacterAnimation characterAnimation;
        protected CharacterExperience characterExperience;
        protected CreepEndurance creepEndurance;
        protected CreepHealth creepHealth;
        protected CreepMana creepMana;
        protected Gravity gravity;
        protected CreepUI creepUI;

        public CreepStateFactory CreepStateFactory { get; private set; }
        
        protected abstract CreepStateFactory CreateCreepStateFactory();


        public override void Initialize()
        {
            base.Initialize();
            
            gravity = GetComponentInUnit<Gravity>();
            gravity.InActivateGravity();
        }

        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();
            navMeshAgent = GetComponentInUnit<NavMeshAgent>();
            characterAnimation = GetComponentInUnit<CharacterAnimation>();
            characterAnimation.Initialize();
            
            CreepStateFactory = CreateCreepStateFactory();
            
            creepUI = GetComponentInUnit<CreepUI>();
            diContainer.Inject(creepUI);
            creepUI.Initialize();
        }

        protected override void AfterInitializeMediator()
        {
            base.AfterInitializeMediator();
            
            creepHealth = GetComponentInUnit<CreepHealth>();
            diContainer.Inject(creepHealth);
            creepHealth.Initialize();
            
            characterExperience = GetComponentInUnit<CharacterExperience>();
            diContainer.Inject(characterExperience);
            characterExperience.Initialize();
            
            creepEndurance = GetComponentInUnit<CreepEndurance>();
            diContainer.Inject(creepEndurance);
            creepEndurance.Initialize();
            
            creepMana = GetComponentInUnit<CreepMana>();
            diContainer.Inject(creepMana);
            creepMana.Initialize();
        }

        public override void Activate()
        {
            base.Activate();
            navMeshAgent.enabled = true;
            gravity.ActivateGravity();
        }

        protected override void InitializeMediatorEvent()
        {
            base.InitializeMediatorEvent();
            this.StateMachine.OnChangedState += OnChangedState;
            GetComponentInUnit<CreepHealth>().OnZeroHealth += OnZeroHealth;
            GetComponentInUnit<CreepHealth>().OnTakeDamage += GetComponentInUnit<CreepUI>().OnTakeDamage;
            GetComponentInUnit<CreepEndurance>().EnduranceStat.OnChangedCurrentValue += OnChangedEndurance;
            GetComponentInUnit<CreepEndurance>().EnduranceStat.OnChangedMaximumValue += OnChangedEndurance;
            GetComponentInUnit<CreepHealth>().HealthStat.OnChangedCurrentValue += OnChangedHealth;
            GetComponentInUnit<CreepHealth>().HealthStat.OnChangedMaximumValue += OnChangedHealth;
            GetComponentInUnit<CreepMana>().ManaStat.OnChangedCurrentValue += OnChangedMana;
            GetComponentInUnit<CreepMana>().ManaStat.OnChangedMaximumValue += OnChangedMana;
        }

        protected override void DeInitializeMediatorEvent()
        {
            base.DeInitializeMediatorEvent();
            this.StateMachine.OnChangedState -= OnChangedState;
            GetComponentInUnit<CreepHealth>().OnZeroHealth -= OnZeroHealth;
            GetComponentInUnit<CreepHealth>().OnTakeDamage -= GetComponentInUnit<CreepUI>().OnTakeDamage;
            GetComponentInUnit<CreepEndurance>().EnduranceStat.OnChangedCurrentValue -= OnChangedEndurance;
            GetComponentInUnit<CreepEndurance>().EnduranceStat.OnChangedMaximumValue -= OnChangedEndurance;
            GetComponentInUnit<CreepHealth>().HealthStat.OnChangedCurrentValue -= OnChangedHealth;
            GetComponentInUnit<CreepHealth>().HealthStat.OnChangedMaximumValue -= OnChangedHealth;
            GetComponentInUnit<CreepMana>().ManaStat.OnChangedCurrentValue -= OnChangedMana;
            GetComponentInUnit<CreepMana>().ManaStat.OnChangedMaximumValue -= OnChangedMana;
        }

        private void OnChangedEndurance()
        {
            creepUI.OnChangedEndurance(creepEndurance.EnduranceStat.CurrentValue, creepEndurance.EnduranceStat.MaximumValue);
        }
        private void OnChangedHealth()
        {
            creepUI.OnChangedHealth(creepHealth.HealthStat.CurrentValue, creepHealth.HealthStat.MaximumValue);
        }

        private void OnChangedMana()
        {
            creepUI.OnChangedMana(creepMana.ManaStat.CurrentValue, creepMana.ManaStat.MaximumValue);
        }
        
        protected void Update()
        {
            if(!IsActive) return;
            this.StateMachine?.Update();
        }

        protected void LateUpdate()
        {
            if(!IsActive) return;
            this.StateMachine?.LateUpdate();
        }
        
        protected void OnChangedState(IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }

        private void OnZeroHealth()
        {
            characterExperience.GiveExperience();
            OnDeath?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}