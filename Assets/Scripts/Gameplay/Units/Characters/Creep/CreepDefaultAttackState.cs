using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepDefaultAttackState : CharacterDefaultAttackState
    {
        protected NavMeshAgent navMeshAgent;
        protected CreepIdleState creepIdleState;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        
        public override void Enter()
        {
            base.Enter();
            creepIdleState ??= stateMachine.GetState<CreepIdleState>();
            navMeshAgent.updateRotation = false;
        }

        public override void Exit()
        {
            base.Exit();
            navMeshAgent.updateRotation = true;
        }

        protected override void FindUnit()
        {
            base.FindUnit();
            creepIdleState.SetTarget(currentTarget);
        }
    }
    
    public class CreepDefaultAttackStateBuilder : CharacterDefaultAttackStateBuilder
    {
        public CreepDefaultAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
        }

        public CreepDefaultAttackStateBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(state is CreepDefaultAttackState creepDefaultAttackState)
                creepDefaultAttackState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
    }
}