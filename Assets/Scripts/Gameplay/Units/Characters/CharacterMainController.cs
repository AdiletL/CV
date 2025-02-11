using System;
using System.Collections.Generic;
using Photon.Pun;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterMainController : UnitController, ICharacter, IClickableObject
    {
        [field: SerializeField] public SO_CharacterInformation SO_CharacterInformation { get; private set; }

        protected PhotonView photonView;
        
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
            
            photonView = GetComponent<PhotonView>();
            
            StateMachine = new StateMachine();
            UnitInformation = CreateUnitInformation();
            diContainer.Inject(UnitInformation);
            
            var ui = GetComponentInUnit<CharacterUI>();
            diContainer.Inject(ui);
            ui?.Initialize();
            
            InitializeMediator();
            
            BeforeCreateStates();
            CreateSwitchState();
            CreateStates();
            AfterCreateStates();
            
            AfterInitializeMediator();
            
            InitializeAllAnimations();
        }


        protected virtual void InitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth += GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnDeath += GetComponentInUnit<CharacterExperience>().OnDeath;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance += GetComponentInUnit<CharacterUI>().OnChangedEndurance;
        }
        protected virtual void UnInitializeMediatorRPC()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth -= GetComponentInUnit<CharacterUI>().OnChangedHealth;
            GetComponentInUnit<CharacterHealth>().OnDeath -= GetComponentInUnit<CharacterExperience>().OnDeath;
            GetComponentInUnit<CharacterEndurance>().OnChangedEndurance -= GetComponentInUnit<CharacterUI>().OnChangedEndurance;
        }

        protected virtual void BeforeCreateStates()
        {
            
        }
        protected abstract void CreateStates();
        protected abstract void CreateSwitchState();

        protected virtual void AfterCreateStates()
        {
            
        }

        protected virtual void AfterInitializeMediator()
        {
            var health = GetComponentInUnit<CharacterHealth>();
            diContainer.Inject(health);
            health?.Initialize();
        }

        protected abstract void InitializeAllAnimations();

        protected virtual void OnDestroy()
        {
            UnInitializeMediatorRPC();
        }

        public void ShowInformation() => UnitInformation.Show();
        public void UpdateInformation() => UnitInformation.UpdateData();
        public void HideInformation() => UnitInformation.Hide();
    }
}
