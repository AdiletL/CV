using System;
using System.Collections.Generic;
using Gameplay.Equipment;
using Gameplay.Weapon;
using Photon.Pun;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterMainController : UnitController, IClickableObject
    {
        [field: SerializeField] public SO_CharacterInformation SO_CharacterInformation { get; private set; }

        protected PhotonView photonView;

        protected List<Equipment> currentEquipments;
        
        public StateMachine StateMachine { get; protected set; }
        public UnitInformation UnitInformation { get; protected set; }
        
        public bool IsNullEquipment(Equipment equipment) => 
            currentEquipments == null || !currentEquipments.Contains(equipment);

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
            
            SubscribeEvent();
            
            BeforeCreateStates();
            CreateStates();
            StateMachine.Initialize();
            
            AfterCreateStates();
            AfterInitializeMediator();
            InitializeAllAnimations();
        }


        protected virtual void SubscribeEvent()
        {
            
        }
        protected virtual void UnSubscribeEvent()
        {
           
        }

        protected virtual void BeforeCreateStates()
        {
            
        }
        protected abstract void CreateStates();

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

        public virtual void PutOnEquipment(Equipment equipment)
        {
            currentEquipments ??= new List<Equipment>();
            currentEquipments.Add(equipment);
        }

        public virtual void TakeOffEquipment(Equipment equipment)
        {
            currentEquipments?.Remove(equipment);
        }

        public void ShowInformation() => UnitInformation.Show();
        public void UpdateInformation() => UnitInformation.UpdateData();
        public void HideInformation() => UnitInformation.Hide();
        
        protected virtual void OnDestroy()
        {
            UnSubscribeEvent();
        }
    }
}
