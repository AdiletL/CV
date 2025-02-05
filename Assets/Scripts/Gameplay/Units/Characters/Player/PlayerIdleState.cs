using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        private CharacterSwitchMoveState characterSwitchMove;
        private CharacterSwitchAttackState characterSwitchAttack;

        private GameObject targetForMove;
        private Vector3 targetPosition;

        
        public void SetCharacterSwitchMoveState(CharacterSwitchMoveState characterSwitchMoveState) => characterSwitchMove = characterSwitchMoveState;
        public void SetCharacterSwitchAttackState(CharacterSwitchAttackState characterSwitchAttackState) =>
            characterSwitchAttack = characterSwitchAttackState;
        

        public override void Subscribe()
        {
            base.Subscribe();
            this.stateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            CheckMove();
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            this.stateMachine.OnExitCategory -= OnExitCategory;
        }

        public override void Exit()
        {
            base.Exit();
            targetForMove = null;
        }
        
        private void OnExitCategory(Machine.IState state)
        {
            if(!isActive) return;
            
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                PlayAnimation();
            }
        }
        
        public void SetTarget(GameObject target)
        {
            this.targetForMove = target;
            characterSwitchMove.SetTarget(target);
        }
        
        private void CheckMove()
        {
            if(!targetForMove) return;

            targetPosition = new Vector3(targetForMove.transform.position.x, gameObject.transform.position.y, targetForMove.transform.position.z);
            
            if (Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, targetPosition))
            {
                targetForMove = null;
            }
            else
            {
                characterSwitchMove.ExitCategory(Category);
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