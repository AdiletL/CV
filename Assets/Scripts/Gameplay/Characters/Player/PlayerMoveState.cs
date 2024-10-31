using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PlayerMoveState : CharacterMoveState
    {
        private GameObject currentTarget;
        private List<Type> currentStates = new List<Type>();

        public Type runState;

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
                MoveStateMachine.SetStates(runState);
            }
            else
            {
                this.StateMachine.SetStates(typeof(PlayerIdleState));
            }
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerMoveState())
        {
        }

        public PlayerMoveStateBuilder SetRunState(IState runState)
        {
            if (state is PlayerMoveState playerMoveState)
            {
                playerMoveState.runState = runState.GetType();
            }

            return this;
        }
    }
}