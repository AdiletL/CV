using System.Collections.Generic;
using Calculate;
using Movement;
using Unit.Cell;
using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepRunState : CharacterRunState
    {
        protected NavMeshAgent navMeshAgent;
        protected GameObject currentTarget;

        protected float rotationSpeed;

        protected const float stoppingDistance = .2f;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetRotationSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;

        
        public override void Initialize()
        {
            base.Initialize();
            navMeshAgent.speed = MovementSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }

        public override void Enter()
        {
            base.Enter();
            navMeshAgent.speed = MovementSpeed;
            navMeshAgent.destination = currentTarget.transform.position;
            navMeshAgent.isStopped = false;
            PlayAnimation();
        }

        public override void Update()
        {
            base.Update();
            ExecuteMovement();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            if (isActive)
            {
                navMeshAgent.destination = currentTarget.transform.position;
                PlayAnimation();
            }
        }

        public override void ExecuteMovement()
        {
            base.ExecuteMovement();
            if (navMeshAgent.isOnNavMesh &&
                !navMeshAgent.pathPending &&
                navMeshAgent.remainingDistance < stoppingDistance)
            {
                StateMachine.ExitCategory(Category, null);
            }
        }
    }

public class CreepRunStateBuilder : CharacterRunStateBuilder
    {
        public CreepRunStateBuilder(CreepRunState instance) : base(instance)
        {
        }


        public CreepRunStateBuilder SetNavMesh(NavMeshAgent navMeshAgent)
        {
            if (state is CreepRunState creepRunState)
                creepRunState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
        
        public CreepRunStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is CreepRunState creepRunState)
                creepRunState.SetRotationSpeed(rotationSpeed);
            return this;
        }
    }
}
