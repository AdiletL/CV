using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderPatrolState : CreepPatrolState
    {
        private float countCooldownCheckEnemy;
        private const float COOLDOWN_CHECK_ENEMY = .3f;
        
        public override void Update()
        {
            base.Update();
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