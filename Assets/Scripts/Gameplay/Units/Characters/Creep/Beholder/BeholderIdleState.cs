using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 
        private float countCheckEnemyCooldown;
        private const float CHECK_ENEMY_COOLDOWN = .03f;
        

        public override void Update()
        {
            base.Update();
            if (currentTarget != null)
            {
                stateMachine.GetState<CreepAttackState>().SetTarget(currentTarget);
                stateMachine.ExitCategory(Category, typeof(CreepAttackState));
                return;
            }
            
            CheckEnemy();
        }
        
        public override void Exit()
        {
            base.Exit();
            countCheckEnemyCooldown = 0;
        }

        private void CheckEnemy()
        {
            if(!IsActive) return;
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > CHECK_ENEMY_COOLDOWN)
            {
                if (stateMachine.GetState<CreepAttackState>().IsFindUnitInRange())
                    stateMachine.ExitCategory(Category, typeof(CreepAttackState));
                else
                    stateMachine.ExitCategory(Category, typeof(CreepMoveState));

                countCheckEnemyCooldown = 0;
            }
        }
    }

    public class BeholderIdleStateBuilder : CreepIdleStateBuilder
    {
        public BeholderIdleStateBuilder() : base(new BeholderIdleState())
        {
        }
    }
}