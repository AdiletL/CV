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

        [field: SerializeField, Space] public GameObject SelectedObjectVisual { get; private set; }

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

            BeforeCreateStates();
            
            CreateSwitchState();
            CreateStates();

            BeforeInitializeMediator();
            InitializeMediator();

            var ui = GetComponentInUnit<CharacterUI>();
            var health = GetComponentInUnit<CharacterHealth>();
            
            diContainer.Inject(ui);
            diContainer.Inject(health);
            
            ui?.Initialize();
            health?.Initialize();

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
            photonView = GetComponent<PhotonView>();
            
            StateMachine = new StateMachine();
            UnitInformation = CreateUnitInformation();
            diContainer.Inject(UnitInformation);
        }
        protected abstract void CreateStates();
        protected abstract void CreateSwitchState();

        protected virtual void BeforeInitializeMediator()
        {
            
        }

        protected abstract void InitializeAllAnimations();

        protected virtual void OnDestroy()
        {
            UnInitializeMediatorRPC();
        }

        public void ShowInformation() => UnitInformation.Show();
        public void UpdateInformation() => UnitInformation.UpdateData();
        public void HideInformation() => UnitInformation.Hide();
        public void SelectObject() => SelectedObjectVisual.SetActive(true);
        public void UnSelectObject() => SelectedObjectVisual.SetActive(false);
    }
}
