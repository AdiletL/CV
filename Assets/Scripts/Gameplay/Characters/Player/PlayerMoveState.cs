using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        private GameObject currentTarget;

        public PlayerRunState runState;

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
                MoveStateMachine.SetStates(runState.GetType());
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
            if (runState is PlayerRunState playerRunState)
            {
                if (state is PlayerMoveState playerMoveState)
                {
                    playerMoveState.MoveStateMachine.AddState(playerRunState);
                    playerMoveState.runState = playerRunState;
                }
            }

            return this;
        }
    }
}