using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepAttackState : CharacterAttackState
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

        protected virtual void FindUnit()
        {
            creepIdleState.SetTarget(currentTarget);
        }
    }
    
    public class CreepAttackStateBuilder : CharacterAttackStateBuilder
    {
        public CreepAttackStateBuilder(CreepAttackState instance) : base(instance)
        {
        }

        public CreepAttackStateBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(state is CreepAttackState creepDefaultAttackState)
                creepDefaultAttackState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
    }
}