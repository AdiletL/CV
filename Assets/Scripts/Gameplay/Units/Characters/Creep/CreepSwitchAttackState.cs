using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepSwitchAttackState : CharacterSwitchAttackState
    {
        protected NavMeshAgent navMeshAgent;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        
        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<ICreepAttackable>(center.position, RangeAttack, enemyLayer, ref findUnitColliders);
        }
    }

    public class CreepSwitchAttackStateBuilder : CharacterSwitchAttackStateBuilder
    {
        public CreepSwitchAttackStateBuilder(CharacterSwitchAttackState instance) : base(instance)
        {
        }

        public CreepSwitchAttackStateBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(switchState is CreepSwitchAttackState creepSwitchAttackState)
                creepSwitchAttackState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
    }
}