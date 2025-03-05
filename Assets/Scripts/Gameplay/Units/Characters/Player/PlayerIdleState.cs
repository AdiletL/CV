using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        private CharacterSwitchMoveState characterSwitchMove;
        private CharacterSwitchAttackState characterSwitchAttack;

        private Vector3 targetPosition;

        
        public void SetCharacterSwitchMoveState(CharacterSwitchMoveState characterSwitchMoveState) => characterSwitchMove = characterSwitchMoveState;
        public void SetCharacterSwitchAttackState(CharacterSwitchAttackState characterSwitchAttackState) =>
            characterSwitchAttack = characterSwitchAttackState;
        

        public override void Subscribe()
        {
            base.Subscribe();
            this.stateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            this.stateMachine.OnExitCategory -= OnExitCategory;
        }

        private void OnExitCategory(IState state)
        {
            if(!IsActive) return;
            
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                PlayAnimation();
            }
        }
    }
    

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleState())
        {
            
        }

        public PlayerIdleStateBuilder SetCharacterSwitchMove(CharacterSwitchMoveState characterSwitchMove)
        {
            if(state is PlayerIdleState playerIdleState)
                playerIdleState.SetCharacterSwitchMoveState(characterSwitchMove);

            return this;
        }
    }
}