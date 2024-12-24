using System;
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

        public virtual void IncreaseStates(params IState[] stats)
        {
            foreach (IState state in stats)
            {
                Debug.Log(state.StateType);
                switch (state.StateType)
                {
                    case StateType.nothing:
                        break;
                    case StateType.health:
                        components.GetComponentFromArray<CharacterHealth>()?.IncreaseStates(state);
                        break;
                    case StateType.movement:
                        StateMachine.GetState<CharacterSwitchMoveState>()?.IncreaseStates(state);
                        break;
                    case StateType.attack:
                        StateMachine.GetState<CharacterSwitchAttackState>()?.IncreaseStates(state);
                        break;
                    case StateType.level:
                        components.GetComponentFromArray<CharacterExperience>()?.IncreaseLevel(state);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
