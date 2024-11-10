using System;
using System.Collections.Generic;
using ScriptableObjects.Character.Player;
using UnityEngine;

namespace Character.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        private SO_PlayerMove so_PlayerMove;
        private GameObject currentTarget;


        public override void Initialize()
        {
            base.Initialize();
            so_PlayerMove = (SO_PlayerMove)SO_CharacterMove;
        }

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        protected override void DestermineState()
        {
            //TODO: Select type move

            if (!movementStates.ContainsKey(typeof(PlayerRunState)))
            {
                var runState = (PlayerRunState)new PlayerRunStateBuilder()
                    .SetCharacterAnimation(this.CharacterAnimation)
                    .SetAnimationClip(so_PlayerMove.RunClip)
                    .SetGameObject(GameObject)
                    .SetMovementSpeed(so_PlayerMove.RunSpeed)
                    .SetStateMachine(this.StateMachine)
                    .Build();
                
                runState.Initialize();
                runState.SetTarget(currentTarget);
                movementStates.TryAdd(typeof(PlayerRunState), runState);
                this.StateMachine.AddStates(runState);
            }

            if (movementStates.TryGetValue(typeof(PlayerRunState), out var state)
                && state is PlayerRunState playerRunState)
            {
                playerRunState.SetTarget(currentTarget);
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