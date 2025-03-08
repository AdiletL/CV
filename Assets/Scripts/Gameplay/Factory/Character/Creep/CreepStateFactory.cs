using System;
using Gameplay.Unit.Character;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Factory.Character.Creep
{
    public abstract class CreepStateFactory : CharacterStateFactory
    {
        protected NavMeshAgent navMeshAgent;
        protected StateMachine stateMachine;
        protected CharacterAnimation characterAnimation;
        protected Vector3[] patrolPoints;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetPatrolPoints(Vector3[] patrolPoints) => this.patrolPoints = patrolPoints;
    }
    
    public class CreepStateFactoryBuilder : CharacterStateFactoryBuilder
    {
        public CreepStateFactoryBuilder(CharacterStateFactory factory) : base(factory)
        {
        }

        public CreepStateFactoryBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(factory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetNavMeshAgent(navMeshAgent);
            return this;
        }

        public CreepStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            if(factory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetStateMachine(stateMachine);
            return this;
        }
        
        public CreepStateFactoryBuilder SetPatrolPoints(Vector3[] patrolPoints)
        {
            if(factory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetPatrolPoints(patrolPoints);
            return this;
        }
        
        public CreepStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(factory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}