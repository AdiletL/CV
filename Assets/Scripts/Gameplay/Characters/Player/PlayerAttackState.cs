using UnityEngine;

namespace Character.Player
{
    public class PlayerAttackState : CharacterAttackState
    {
        private GameObject currentTarget;
        
        protected override void DestermineState()
        {
            var enemyGameObject = CheckForwardEnemy();
            if (enemyGameObject)
            {
                this.StateMachine.GetState<PlayerMoveState>().Rotate(enemyGameObject);
                if (this.StateMachine.GetState<PlayerMoveState>().IsLookAtTarget())
                {
                    currentTarget = enemyGameObject;
                    AttackStateMachine.GetState<PlayerMeleeAttackState>().SetTarget(enemyGameObject);
                    AttackStateMachine.SetStates(typeof(PlayerMeleeAttackState));
                }
            }
            else
            {
                this.StateMachine.SetStates(typeof(PlayerIdleState));
            }
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            AttackStateMachine.GetState<PlayerMeleeAttackState>()?.SetTarget(currentTarget);
            Debug.Log(AttackStateMachine.GetState<PlayerMeleeAttackState>());
        }
    }

    public class PlayerAttackStateBuilder : CharacterAttackStateBuilder
    {
        public PlayerAttackStateBuilder() : base(new PlayerAttackState())
        {
        }

        public PlayerAttackStateBuilder SetMeleeAttackState(IState meleeState)
        {
            if (meleeState is PlayerMeleeAttackState playerMeleeState)
            {
                if (state is PlayerAttackState playerAttackState)
                {
                    state.AttackStateMachine.AddState(playerMeleeState);
                }
            }

            return this;
        }
    }
}