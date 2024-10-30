using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterMoveState : State
    {
        public GameObject GameObject;
        public StateMachine MoveStateMachine;
        
        public override void Enter()
        {
            DestermineState();
        }

        public override void Update()
        {
            DestermineState();
            MoveStateMachine?.Update();
        }

        public override void Exit()
        {
            MoveStateMachine.SetStates(new List<Type>(){ typeof(CharacterMoveState) });
        }

        protected virtual void DestermineState()
        {
            
        }
    }

    public class CharacterMoveStateBuilder : StateBuilder<CharacterMoveState>
    {
        public CharacterMoveStateBuilder(CharacterMoveState instance) : base(instance)
        {
        }

        public CharacterMoveStateBuilder SetStates(IState[] moveStates)
        {
            state.MoveStateMachine = new StateMachine();
            foreach (var VARIABLE in moveStates)
            {
                state.MoveStateMachine.AddState(VARIABLE);
            }

            return this;
        }

        public CharacterMoveStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }
    }
}