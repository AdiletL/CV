using System;
using Gameplay.Factory;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMoveState : CharacterSwitchMoveState
    {
        private CharacterRunState characterRunState;
        private PlayerStateFactory playerStateFactory;
        
        public void SetPlayerStateFactory(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        

        public void InitializeRunStateOrig()
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
            InitializeRunStateOrig();
            if(!this.stateMachine.IsActivateType(characterRunState.GetType()))
                this.stateMachine.SetStates(desiredStates: characterRunState.GetType());
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            InitializeRunStateOrig();
            if(!this.stateMachine.IsActivateType(characterRunState.GetType()))
                this.stateMachine.ExitCategory(category, characterRunState.GetType());
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            InitializeRunStateOrig();
            if(!this.stateMachine.IsActivateType(characterRunState.GetType()))
                this.stateMachine.ExitOtherStates(characterRunState.GetType());
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