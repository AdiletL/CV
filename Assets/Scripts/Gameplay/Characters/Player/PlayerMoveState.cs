using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        private GameObject currentTarget;
        
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            this.StateMachine.GetState<PlayerRunState>()?.SetTarget(currentTarget);
        }
        
        protected override void DestermineState()
        {
            //TODO: Select type move
            
            var playerRunState = this.StateMachine.GetState<PlayerRunState>();
            if(playerRunState == null) return;
            if (currentMoveState != playerRunState)
            {
                playerRunState.SetTarget(currentTarget);
                currentMoveState = playerRunState;
                currentMoveState.Initialize();
            }
            
            this.StateMachine.SetStates(typeof(PlayerRunState));
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerMoveState())
        {
        }
        
    }
}