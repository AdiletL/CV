using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderPatrolState : CreepAgentPatrolState
    {
        private CreepSwitchAttackState creepSwitchAttackState;

        public void SetCreepSwitchAttackState(CreepSwitchAttackState creepSwitchAttackState) =>
            this.creepSwitchAttackState = creepSwitchAttackState;
        
        
        public override void LateUpdate()
        {
            //CheckEnemy();
        }

        private void CheckEnemy()
        {
            if (creepSwitchAttackState.IsFindUnitInRange())
            {
                creepSwitchAttackState.ExitCategory(Category);
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