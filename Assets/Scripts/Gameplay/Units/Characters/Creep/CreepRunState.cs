using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepRunState : CharacterRunState
    {
        protected NavMeshAgent navMeshAgent;
        protected GameObject currentTarget;
        protected CreepIdleState creepIdleState;

        protected float rotationSpeed;
        protected float timerRunToTarget;
        protected float countTimerRunToTarget;
        protected float countCooldownUpdateTargetPosition;

        protected const float cooldownUpdateTargetPosition = .5f;
        protected const float stoppingDistance = .2f;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetRotationSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;
        public void SetTimerRunToTarget(float timerRunToTarget) => this.timerRunToTarget = timerRunToTarget;

        
        public override void Initialize()
        {
            base.Initialize();
            navMeshAgent.speed = CurrentMovementSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }

        public override void Enter()
        {
            base.Enter();
            creepIdleState ??= stateMachine.GetState<CreepIdleState>();
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.speed = CurrentMovementSpeed;
                navMeshAgent.destination = currentTarget.transform.position;
                navMeshAgent.isStopped = false;
            }
            PlayAnimation();
        }

        public override void Update()
        {
            base.Update();
            ExecuteMovement();
            CheckTimerRunToTarget();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }
        }

        private void ClearValues()
        {
            countTimerRunToTarget = 0;
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            if (IsActive)
            {
                navMeshAgent.destination = currentTarget.transform.position;
                PlayAnimation();
            }
        }

        public override void ExecuteMovement()
        {
            base.ExecuteMovement();
            if (navMeshAgent.isOnNavMesh)
            {
                TimerUpdateTargetPosition();
                
                if (!navMeshAgent.pathPending &&
                    navMeshAgent.remainingDistance < stoppingDistance)
                {
                    stateMachine.ExitCategory(Category, null);
                }
            }
        }

        protected virtual void TimerUpdateTargetPosition()
        {
            countCooldownUpdateTargetPosition += Time.deltaTime;
            if (countCooldownUpdateTargetPosition > cooldownUpdateTargetPosition)
            {
                navMeshAgent.destination = currentTarget.transform.position;
                countCooldownUpdateTargetPosition = 0;
            }
        }
        
        protected virtual void CheckTimerRunToTarget()
        {
            countTimerRunToTarget += Time.deltaTime;
            if (countTimerRunToTarget >= timerRunToTarget)
            {
                creepIdleState.SetTarget(null);
                stateMachine.ExitCategory(Category, null);
                countTimerRunToTarget = 0;
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
        
        public CreepRunStateBuilder SetTimerRunToTarget(float value)
        {
            if (state is CreepRunState creepRunState)
                creepRunState.SetTimerRunToTarget(value);
            return this;
        }
    }
}
