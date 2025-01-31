using System;
using Gameplay.Factory;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMoveState : CharacterSwitchMoveState
    {
        private SO_PlayerMove so_PlayerMove;
        private CharacterRunState characterRunState;
        private PlayerStateFactory playerStateFactory;
        
        
        public void SetPlayerStateFactory(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        
        public override void Initialize()
        {
            base.Initialize();
            so_PlayerMove = (SO_PlayerMove)so_CharacterMove;
        }

        public void InitializeRunState()
        {
            if (!this.stateMachine.IsStateNotNull(typeof(PlayerRunState)))
            {
                characterRunState = (PlayerRunState)playerStateFactory.CreateState(typeof(PlayerRunState));
                characterRunState.Initialize();
                this.stateMachine.AddStates(characterRunState);
            }
        }
        public override void SetState()
        {
            base.SetState();
            if (currentTarget)
            {
                InitializeRunState();

                characterRunState.SetTarget(currentTarget);
                if(!this.stateMachine.IsActivateType(characterRunState.GetType()))
                    this.stateMachine.SetStates(desiredStates: characterRunState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentTarget)
            {
                InitializeRunState();
                
                characterRunState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterRunState.GetType()))
                    this.stateMachine.ExitCategory(category, characterRunState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (currentTarget)
            {
                InitializeRunState();

                characterRunState.SetTarget(currentTarget);
                if (!this.stateMachine.IsActivateType(characterRunState.GetType()))
                    this.stateMachine.ExitOtherStates(characterRunState.GetType());
            }
            currentTarget = null;
        }
    }

    public class PlayerSwitchMoveStateBuilder : CharacterSwitchMoveStateBuilder
    {
        public PlayerSwitchMoveStateBuilder() : base(new PlayerSwitchMoveState())
        {
        }
        
        public PlayerSwitchMoveStateBuilder SetPlayerStateFactory(PlayerStateFactory playerStateFactory)
        {
            if (switchState is PlayerSwitchMoveState playerSwitchAttackState)
                playerSwitchAttackState.SetPlayerStateFactory(playerStateFactory);

            return this;
        }
    }
}