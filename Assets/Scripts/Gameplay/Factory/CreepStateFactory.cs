using System;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Unit.Character;
using Unit.Character.Creep;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Factory
{
    public abstract class CreepStateFactory : CharacterStateFactory
    {
        protected NavMeshAgent navMeshAgent;
        protected StateMachine stateMachine;
        protected CharacterAnimation characterAnimation;
        protected SO_CreepMove so_CreepMove;
        protected Vector3[] patrolPoints;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCreepMoveConfig(SO_CreepMove so_CreepMove) => this.so_CreepMove = so_CreepMove;
        public void SetPatrolPoints(Vector3[] patrolPoints) => this.patrolPoints = patrolPoints;
        
    }
    
    public class CreepStateFactoryBuilder : CharacterStateFactoryBuilder
    {
        public CreepStateFactoryBuilder(CharacterStateFactory characterStateFactory) : base(characterStateFactory)
        {
        }

        public CreepStateFactoryBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(characterStateFactory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetNavMeshAgent(navMeshAgent);
            return this;
        }

        public CreepStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            if(characterStateFactory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetStateMachine(stateMachine);
            return this;
        }

        public CreepStateFactoryBuilder SetCreepMoveConfig(SO_CreepMove so_CreepMove)
        {
            if(characterStateFactory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetCreepMoveConfig(so_CreepMove);
            return this;
        }
        
        public CreepStateFactoryBuilder SetPatrolPoints(Vector3[] patrolPoints)
        {
            if(characterStateFactory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetPatrolPoints(patrolPoints);
            return this;
        }
        
        public CreepStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(characterStateFactory is CreepStateFactory creepStateFactory)
                creepStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}