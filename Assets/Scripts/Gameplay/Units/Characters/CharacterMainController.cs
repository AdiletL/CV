using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterMainController : UnitController, ICharacterController
    {
        public StateMachine StateMachine { get; protected set; }
        

        public T GetState<T>() where T : Machine.IState
        {
            return StateMachine.GetState<T>();
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
            
            GetComponentInUnit<CharacterUI>()?.Initialize();
            GetComponentInUnit<IUnitExperience>()?.Initialize();
        }

        private void InitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth += GetComponentInUnit<CharacterUI>().OnChangeHealth;
        }

        protected virtual void DeInitializeMediator()
        {
            GetComponentInUnit<CharacterHealth>().OnChangedHealth -= GetComponentInUnit<CharacterUI>().OnChangeHealth;
        }

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
