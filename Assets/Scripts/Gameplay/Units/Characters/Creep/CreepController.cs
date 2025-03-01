﻿using System;
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
        public event Action<CreepController> OnDeath; 
        
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected NavMeshAgent navMeshAgent;
        protected CreepSwitchStateFactory creepSwitchStateFactory;
        protected CharacterAnimation characterAnimation;
        protected CharacterEndurance characterEndurance;
        protected CharacterExperience characterExperience;
        protected Gravity gravity;

        public CreepStateFactory CreepStateFactory { get; private set; }
        
        protected abstract CreepStateFactory CreateCreepStateFactory();
        protected abstract CreepSwitchStateFactory CreateCreepSwitchStateFactory();


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
            CreepStateFactory.Initialize();
            creepSwitchStateFactory = CreateCreepSwitchStateFactory();
            creepSwitchStateFactory.Initialize();
        }

        protected override void AfterInitializeMediator()
        {
            base.AfterInitializeMediator();
            characterExperience = GetComponentInUnit<CharacterExperience>();
            diContainer.Inject(characterExperience);
            characterExperience.Initialize();
            
            characterEndurance = GetComponentInUnit<CharacterEndurance>();
            diContainer.Inject(characterEndurance);
            characterEndurance.Initialize();
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
            GetComponentInUnit<CreepHealth>().OnZeroHealth += OnZeroHealth;
            GetComponentInUnit<CreepHealth>().OnTakeDamage += GetComponentInUnit<CreepUI>().OnTakeDamage;
        }

        protected override void DeInitializeMediatorRPC()
        {
            base.DeInitializeMediatorRPC();
            this.StateMachine.OnChangedState -= OnChangedState;
            GetComponentInUnit<CreepHealth>().OnZeroHealth -= OnZeroHealth;
            GetComponentInUnit<CreepHealth>().OnTakeDamage -= GetComponentInUnit<CreepUI>().OnTakeDamage;
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

        private void OnZeroHealth()
        {
            OnDeath?.Invoke(this);
        }
    }
}