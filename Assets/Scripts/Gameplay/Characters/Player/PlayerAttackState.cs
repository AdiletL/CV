using UnityEngine;

namespace Character.Player
{
    public class PlayerAttackState : CharacterAttackState
    {
        private GameObject currentTarget;
        
        public PlayerMeleeAttackState meleeState;

        protected override void DestermineState()
        {
            if (currentTarget != null)
            {
                AttackStateMachine.SetStates(meleeState.GetType());
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
                    playerAttackState.meleeState = playerMeleeState;
                }
            }

            return this;
        }
    }
}