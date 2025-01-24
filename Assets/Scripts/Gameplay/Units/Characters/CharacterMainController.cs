using System;
using System.Collections.Generic;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterMainController : UnitController, ICharacter, IClickableObject
    {
        [field: SerializeField] public SO_CharacterInformation SO_CharacterInformation { get; private set; }
        
        public StateMachine StateMachine { get; protected set; }
        public UnitInformation UnitInformation { get; protected set; }
        
        public List<T> GetStates<T>() where T : Machine.IState
        {
            return StateMachine.GetStates<T>();
        }

        public abstract int TotalDamage();
        public abstract int TotalAttackSpeed();
        public abstract float TotalAttackRange();

        protected virtual UnitInformation CreateUnitInformation()
        {
            return new CharacterInformation(this);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            StateMachine = new StateMachine();
            UnitInformation = CreateUnitInformation();
            diContainer.Inject(UnitInformation);
            
            CreateSwitchState();
            CreateStates();

            BeforeInitializeMediator();
            InitializeMediator();

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

        protected virtual void InitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth += GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnDeath += GetComponentInUnit<CharacterExperience>().OnDeath;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance += GetComponentInUnit<CharacterUI>().OnChangedEndurance;
        }

        protected virtual void UnInitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth -= GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnDeath -= GetComponentInUnit<CharacterExperience>().OnDeath;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance -= GetComponentInUnit<CharacterUI>().OnChangedEndurance;
        }

        protected abstract void CreateStates();
        protected abstract void CreateSwitchState();

        protected virtual void BeforeInitializeMediator()
        {
            
        }

        protected virtual void OnEnable()
        {
            this.StateMachine?.ExitOtherStates(typeof(CharacterIdleState));
        }

        protected virtual void OnDestroy()
        {
            UnInitializeMediator();
        }

        public void ShowInformation() => UnitInformation.Show();
        public void UpdateInformation() => UnitInformation.UpdateData();
        public void HideInformation() => UnitInformation.Hide();
    }
}
