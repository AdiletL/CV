﻿using Gameplay.Factory.Character;
using Gameplay.Factory.Character.Player;
using Machine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMoveState : CharacterSwitchMoveState
    {
        private CharacterRunState characterRunState;
        private CharacterStateFactory characterStateFactory;
        
        public void SetCharacterStateFactory(CharacterStateFactory characterStateFactory) => this.characterStateFactory = characterStateFactory;
        

        public void InitializeRunStateOrig()
        {
            if (!this.stateMachine.IsStateNotNull(typeof(PlayerRunState)))
            {
                characterRunState = (PlayerRunState)characterStateFactory.CreateState(typeof(PlayerRunState));
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
        
        public PlayerSwitchMoveStateBuilder SetCharacterStateFactory(CharacterStateFactory characterStateFactory)
        {
            if (switchState is PlayerSwitchMoveState playerSwitchAttackState)
                playerSwitchAttackState.SetCharacterStateFactory(characterStateFactory);

            return this;
        }
    }
}