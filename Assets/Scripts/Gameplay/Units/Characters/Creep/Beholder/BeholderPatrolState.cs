using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderPatrolState : CreepAgentPatrolState
    {
        private float countCooldownCheckEnemy;
        private const float COOLDOWN_CHECK_ENEMY = .3f;
        
        public override void LateUpdate()
        {
            base.LateUpdate();
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > COOLDOWN_CHECK_ENEMY)
            {
                if (stateMachine.GetState<CreepAttackState>().IsFindUnitInRange())
                    stateMachine.ExitCategory(Category, typeof(CreepAttackState));

                countCooldownCheckEnemy = 0;
            }
        }
    }
    
    public class BeholderPatrolStateBuilder : CreepAgentPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }
    }
}