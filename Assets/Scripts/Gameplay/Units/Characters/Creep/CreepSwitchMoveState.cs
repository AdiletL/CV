using Gameplay.Factory.Character.Creep;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepSwitchMoveState : CharacterSwitchMoveState
    {
        protected NavMeshAgent navMeshAgent;
        protected CreepStateFactory creepStateFactory;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetCreepStateFactory(CreepStateFactory creepStateFactory) => this.creepStateFactory = creepStateFactory;
        
        public virtual bool IsCanMovement()
        {
            return true;
        }

    }

    public class CreepSwitchSwitchMoveStateBuilder : CharacterSwitchMoveStateBuilder
    {
        public CreepSwitchSwitchMoveStateBuilder(CharacterSwitchMoveState instance) : base(instance)
        {
        }

        public CreepSwitchSwitchMoveStateBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(switchState is CreepSwitchMoveState creepSwitchMoveState)
                creepSwitchMoveState.SetNavMeshAgent(navMeshAgent);
            return this;
        }

        public CreepSwitchSwitchMoveStateBuilder SetCreepStateFactory(CreepStateFactory creepStateFactory)
        {
            if(switchState is CreepSwitchMoveState creepSwitchMoveState)
                creepSwitchMoveState.SetCreepStateFactory(creepStateFactory);
            return this;
        }
    }
}