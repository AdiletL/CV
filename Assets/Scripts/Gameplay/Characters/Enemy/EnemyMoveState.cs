using UnityEngine;

namespace Character.Enemy
{
    public class EnemyMoveState : CharacterMoveState
    {
        private GameObject currentTarget;

        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        protected override void DestermineState()
        {
            var enemyPatrolState = this.StateMachine.GetState<EnemyPatrolState>();
            if(enemyPatrolState == null) return;
            if (currentMoveState != enemyPatrolState)
            {
                currentMoveState = enemyPatrolState;
                currentMoveState.Initialize();
            }
            
            this.StateMachine.SetStates(typeof(EnemyPatrolState));
        }
    }

    public class EnemyMoveStateBuilder : CharacterMoveStateBuilder
    {
        public EnemyMoveStateBuilder(CharacterMoveState instance) : base(instance)
        {
        }
    }
}