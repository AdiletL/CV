using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Unit.Character
{
    public abstract class CharacterMainController : UnitController, ICharacterController
    {
        protected DiContainer diContainer;
        
        public StateMachine StateMachine { get; protected set; }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        public List<T> GetStates<T>() where T : Machine.IState
        {
            return StateMachine.GetStates<T>();
        }
        
        
        public override void Initialize()
        {
            base.Initialize();
            StateMachine = new StateMachine();
            
            InitializeMediator();

            CreateSwitchState();
            CreateStates();

            var ui = GetComponentInUnit<CharacterUI>();
            var experience = GetComponentInUnit<CharacterExperience>();
            var health = GetComponentInUnit<CharacterHealth>();
            var endurance = GetComponentInUnit<CharacterEndurance>();
            
            diContainer.Inject(ui);
            diContainer.Inject(experience);
            diContainer.Inject(health);
            diContainer.Inject(endurance);
            
            ui?.Initialize();
            experience?.Initialize();
            health?.Initialize();
            endurance?.Initialize();
        }

        private void InitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth += GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnDeath += GetComponentInUnit<CharacterExperience>().OnDeath;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance += GetComponentInUnit<CharacterUI>().OnChangedEndurance;
        }

        protected virtual void DeInitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth -= GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnDeath -= GetComponentInUnit<CharacterExperience>().OnDeath;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance -= GetComponentInUnit<CharacterUI>().OnChangedEndurance;
        }

        protected abstract void CreateStates();
        protected abstract void CreateSwitchState();

        protected virtual void OnEnable()
        {
            this.StateMachine?.ExitOtherStates(typeof(CharacterIdleState));
        }

        protected virtual void OnDestroy()
        {
            DeInitializeMediator();
        }
    }
}
