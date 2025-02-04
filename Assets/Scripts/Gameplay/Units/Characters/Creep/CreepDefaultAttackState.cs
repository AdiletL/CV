using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepDefaultAttackState : CharacterDefaultAttackState
    {
        protected NavMeshAgent navMeshAgent;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;

        public override void Enter()
        {
            base.Enter();
            navMeshAgent.updateRotation = false;
        }

        public override void Exit()
        {
            base.Exit();
            navMeshAgent.updateRotation = true;
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