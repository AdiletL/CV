using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PlayerMoveState : CharacterMoveState
    {
        private GameObject currentTarget;
        
        public List<Type> runStates;

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            MoveStateMachine.GetState<PlayerRunState>()?.SetTarget(target);
        }
        
        protected override void DestermineState()
        {
            if(!currentTarget) return;
            
            if (GameObject.transform.position != currentTarget.transform.position)
            {
                MoveStateMachine.SetStates(runStates);
            }
            else
            {
                this.StateMachine.SetStates(new List<Type>(){ typeof(PlayerIdleState) });
            }
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerMoveState())
        {
        }

        public PlayerMoveStateBuilder SetRunState(Type runState)
        {
            if (state is PlayerMoveState playerMoveState)
            {
                playerMoveState.runStates = new List<Type> { runState };
            }

            return this;
        }
    }
}