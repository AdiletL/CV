using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepAgentPatrolState
    {
        private CreepSwitchAttackState creepSwitchAttackState;

        private float countCooldownCheckEnemy;
        private const float cooldownCheckPlayer = .3f;

        public void SetCreepSwitchAttackState(CreepSwitchAttackState creepSwitchAttackState) =>
            this.creepSwitchAttackState = creepSwitchAttackState;
        
        
        public override void LateUpdate()
        {
            base.LateUpdate();
            CheckEnemy();
        }

        private void CheckEnemy()
        {
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckPlayer)
            {
                if (creepSwitchAttackState.IsFindUnitInRange())
                    creepSwitchAttackState.ExitCategory(Category);

                countCooldownCheckEnemy = 0;
            }
        }
    }
    
    public class BeholderPatrolStateBuilder : CreepAgentPatrolStateBuilder
    {
        public BeholderPatrolStateBuilder() : base(new BeholderPatrolState())
        {
        }

        public BeholderPatrolStateBuilder SetCreepSwitchAttackState(CreepSwitchAttackState creepSwitchAttackState)
        {
            if(state is BeholderPatrolState beholderPatrolState)
                beholderPatrolState.SetCreepSwitchAttackState(creepSwitchAttackState);
            return this;
        }
    }
}