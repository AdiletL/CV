using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterMainController : UnitController, ICharacterController
    {
        public StateMachine StateMachine { get; protected set; }
        
        public override T GetComponentInUnit<T>() where T : class
        {
            return components.GetComponentFromArray<T>();
        }

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
            components.GetComponentFromArray<CharacterUI>()?.Initialize();
            components.GetComponentFromArray<IUnitExperience>()?.Initialize();
        }

        protected virtual void OnEnable()
        {
            this.StateMachine?.ExitOtherStates(typeof(CharacterIdleState));
        }
    }
}
