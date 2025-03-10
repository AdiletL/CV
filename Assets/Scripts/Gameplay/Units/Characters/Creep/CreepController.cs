﻿using System;
using Gameplay.Factory.Character.Creep;
using Machine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Unit.Character.Creep
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class CreepController : CharacterMainController
    {
        public event Action<CreepController> OnDeath; 
        
        [Space]
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;
        
        protected NavMeshAgent navMeshAgent;
        protected CharacterAnimation characterAnimation;
        protected CharacterEndurance characterEndurance;
        protected CharacterExperience characterExperience;
        protected Gravity gravity;

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

        public override void Activate()
        {
            base.Activate();
            navMeshAgent.enabled = true;
            gravity.ActivateGravity();
        }

        protected override void SubscribeEvent()
        {
            base.SubscribeEvent();
            this.StateMachine.OnChangedState += OnChangedState;
            GetComponentInUnit<CreepHealth>().OnZeroHealth += OnZeroHealth;
            GetComponentInUnit<CreepHealth>().OnTakeDamage += GetComponentInUnit<CreepUI>().OnTakeDamage;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance += GetComponentInUnit<CharacterUI>().OnChangedEndurance;
            GetComponentInUnit<CharacterHealth>().OnChangedHealth += GetComponentInUnit<CharacterUI>().OnChangedHealth;
        }

        protected override void UnSubscribeEvent()
        {
            base.UnSubscribeEvent();
            this.StateMachine.OnChangedState -= OnChangedState;
            GetComponentInUnit<CreepHealth>().OnZeroHealth -= OnZeroHealth;
            GetComponentInUnit<CreepHealth>().OnTakeDamage -= GetComponentInUnit<CreepUI>().OnTakeDamage;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance -= GetComponentInUnit<CharacterUI>().OnChangedEndurance;
            GetComponentInUnit<CharacterHealth>().OnChangedHealth -= GetComponentInUnit<CharacterUI>().OnChangedHealth;
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