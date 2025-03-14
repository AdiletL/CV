using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class CreepIdleState : CharacterIdleState
    {
        private float countCheckEnemyCooldown;
        private const float CHECK_ENEMY_COOLDOWN = .03f;

        public override void Update()
        {
            base.Update();
            CheckEnemy();
        }
        
        private void CheckEnemy()
        {
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > CHECK_ENEMY_COOLDOWN)
            {
                if (stateMachine.GetState<CreepAttackState>().IsUnitInRange())
                    stateMachine.ExitCategory(Category, typeof(CreepAttackState));
                else
                    stateMachine.ExitCategory(Category, typeof(CreepPatrolState));

                countCheckEnemyCooldown = 0;
            }
        }

        public override void Exit()
        {
            base.Exit();
            countCheckEnemyCooldown = 0;
        }
    }

    public class CreepIdleStateBuilder : CharacterIdleStateBuilder
    {
        public CreepIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }
        
    }
}