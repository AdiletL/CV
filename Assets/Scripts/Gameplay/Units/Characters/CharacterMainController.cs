using System;
using System.Collections.Generic;
using Photon.Pun;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterMainController : UnitController, IClickableObject
    {
        [field: SerializeField] public SO_CharacterInformation SO_CharacterInformation { get; private set; }

        protected PhotonView photonView;

        public StateMachine StateMachine { get; protected set; }
        
        public override void Initialize()
        {
            base.Initialize();
            
            photonView = GetComponent<PhotonView>();
            StateMachine = new StateMachine();
            
            BeforeCreateStates();
            CreateStates();
            
            InitializeMediatorEvent();
            StateMachine.Initialize();
            
            AfterCreateStates();
            AfterInitializeMediator();
        }


        protected virtual void InitializeMediatorEvent()
        {
            
        }
        protected virtual void DeInitializeMediatorEvent()
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

        }
        
        protected virtual void OnDestroy()
        {
            DeInitializeMediatorEvent();
        }

        public void ShowInformation()
        {
            throw new NotImplementedException();
        }

        public void UpdateInformation()
        {
            throw new NotImplementedException();
        }

        public void HideInformation()
        {
            throw new NotImplementedException();
        }
    }
}
