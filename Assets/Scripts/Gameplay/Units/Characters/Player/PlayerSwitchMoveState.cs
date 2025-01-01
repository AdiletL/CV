using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMoveState : CharacterSwitchMoveState
    {
        private SO_PlayerMove so_PlayerMove;
        private GameObject currentTarget;
        
        public Transform Center { get; set; }

        public CharacterController CharacterController { get; set; }

        private PlayerRunState CreateRunState()
        {
            return (PlayerRunState)new PlayerRunStateBuilder()
                .SetCharacterController(CharacterController)
                .SetMoveConfig(so_PlayerMove)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetCenter(Center)
                .SetCharacterAnimation(this.CharacterAnimation)
                .SetRunClips(so_PlayerMove.RunClip)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_PlayerMove.RunSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            so_PlayerMove = (SO_PlayerMove)SO_CharacterMove;
        }
        
        protected override void DestermineState()
        {
            //TODO: Select type move

            if (!this.StateMachine.IsStateNotNull(typeof(PlayerRunState)))
            {
                var newState = CreateRunState();
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }

            if (!this.StateMachine.IsActivateType(StateCategory.move, typeof(PlayerRunState)))
            {
                var runState = this.StateMachine.GetState<PlayerRunState>();
                runState.SetTarget(currentTarget);
                
                this.StateMachine.SetStates(typeof(PlayerRunState));
            }
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            if (this.StateMachine.IsStateNotNull(typeof(PlayerRunState)))
            {
                this.StateMachine.GetState<PlayerRunState>().SetTarget(currentTarget);
            }
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerSwitchMoveState())
        {
        }
        
        public PlayerMoveStateBuilder SetCenter(Transform center)
        {
            if(state is PlayerSwitchMoveState playerSwitchMoveState)
                playerSwitchMoveState.Center = center;

            return this;
        }
        
        public PlayerMoveStateBuilder SetCharacterController(CharacterController characterController)
        {
            if(state is PlayerSwitchMoveState playerSwitchMoveState)
                playerSwitchMoveState.CharacterController = characterController;

            return this;
        }
    }
}