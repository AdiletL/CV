using System;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMove : CharacterSwitchMove
    {
        private SO_PlayerMove so_PlayerMove;
        private PlayerRunState playerRunState;
        
        public Transform Center { get; set; }

        public CharacterController CharacterController { get; set; }
        public PlayerEndurance PlayerEndurance { get; set; }

        private PlayerRunState CreateRunState()
        {
            return (PlayerRunState)new PlayerRunStateBuilder()
                .SetCharacterController(CharacterController)
                .SetMoveConfig(so_PlayerMove)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetRunDecreaseEndurance(so_PlayerMove.RunDecreaseEndurance)
                .SetPlayerEndurance(PlayerEndurance)
                .SetCenter(Center)
                .SetCharacterAnimation(this.CharacterAnimation)
                .SetCharacterSwitchAttack(this.CharacterSwitchAttack)
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

        public override void SetState()
        {
            base.SetState();
            if (currentTarget)
            {
                if (!this.StateMachine.IsStateNotNull(typeof(PlayerRunState)))
                {
                    playerRunState = CreateRunState();
                    playerRunState.Initialize();
                    this.StateMachine.AddStates(playerRunState);
                }

                playerRunState.SetTarget(currentTarget);
                if(!this.StateMachine.IsActivateType(playerRunState.GetType()))
                    this.StateMachine.SetStates(playerRunState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentTarget)
            {
                if (!this.StateMachine.IsStateNotNull(typeof(PlayerRunState)))
                {
                    playerRunState = CreateRunState();
                    playerRunState.Initialize();
                    this.StateMachine.AddStates(playerRunState);
                }

                playerRunState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerRunState.GetType()))
                    this.StateMachine.ExitCategory(category, playerRunState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (currentTarget)
            {
                if (!this.StateMachine.IsStateNotNull(typeof(PlayerRunState)))
                {
                    playerRunState = CreateRunState();
                    playerRunState.Initialize();
                    this.StateMachine.AddStates(playerRunState);
                }

                playerRunState.SetTarget(currentTarget);
                if (!this.StateMachine.IsActivateType(playerRunState.GetType()))
                    this.StateMachine.ExitOtherStates(playerRunState.GetType());
            }
            currentTarget = null;
        }
    }

    public class PlayerSwitchMoveBuilder : CharacterSwitchMoveBuilder<PlayerSwitchMove>
    {
        public PlayerSwitchMoveBuilder() : base(new PlayerSwitchMove())
        {
        }
        
        public PlayerSwitchMoveBuilder SetCenter(Transform center)
        {
            if(state is PlayerSwitchMove playerSwitchMoveState)
                playerSwitchMoveState.Center = center;

            return this;
        }
        
        public PlayerSwitchMoveBuilder SetCharacterController(CharacterController characterController)
        {
            if(state is PlayerSwitchMove playerSwitchMoveState)
                playerSwitchMoveState.CharacterController = characterController;

            return this;
        }
        public PlayerSwitchMoveBuilder SetPlayerEndurance(PlayerEndurance playerEndurance)
        {
            if(state is PlayerSwitchMove playerSwitchMoveState)
                playerSwitchMoveState.PlayerEndurance = playerEndurance;

            return this;
        }
    }
}