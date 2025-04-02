using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Unit.Character.Creep
{
    public class CreepMoveState : CharacterMoveState
    {
        protected NavMeshAgent navMeshAgent;
        protected GameObject currentTarget;

        protected float rotationSpeed;
        protected float timerRunToTarget;
        protected float countTimerRunToTarget;
        protected float countCooldownUpdateTargetPosition;

        protected const float COOLDOWN_UPDATE_TARGET_POSITION = .5f;
        protected const float STOPPING_DISTANCE = .5f;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetRotationSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;
        public void SetTimerRunToTarget(float timerRunToTarget) => this.timerRunToTarget = timerRunToTarget;

        
        public override void Initialize()
        {
            base.Initialize();
            navMeshAgent.speed = MovementSpeedStat.CurrentValue;
            navMeshAgent.angularSpeed = rotationSpeed;
            navMeshAgent.stoppingDistance = STOPPING_DISTANCE;
        }

        public override void Enter()
        {
            base.Enter();
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.speed = MovementSpeedStat.CurrentValue;
                navMeshAgent.destination = currentTarget.transform.position;
                navMeshAgent.isStopped = false;
            }
            ClearValues();
            currentClip = getRandomClip(runClips);
            PlayAnimation();
            Debug.Log("enter");
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
            Debug.Log("exit");
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
                    navMeshAgent.remainingDistance < STOPPING_DISTANCE &&
                    IsCanMove)
                {
                    CheckUnitInRange();
                }
            }
        }

        protected virtual void TimerUpdateTargetPosition()
        {
            countCooldownUpdateTargetPosition += Time.deltaTime;
            if (countCooldownUpdateTargetPosition > COOLDOWN_UPDATE_TARGET_POSITION)
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
                CheckUnitInRange();
                countTimerRunToTarget = 0;
            }
        }

        protected virtual void CheckUnitInRange()
        {
            if (stateMachine.GetState<CreepAttackState>().IsUnitInRange())
                stateMachine.ExitCategory(Category, typeof(CreepAttackState));
            else
                stateMachine.ExitCategory(Category, null);
            
        }

        public override void ActivateMovement()
        {
            base.ActivateMovement();
            navMeshAgent.isStopped = false;
        }

        public override void DeactivateMovement()
        {
            base.DeactivateMovement();
            navMeshAgent.isStopped = true;
        }
    }

    public class CreepMoveStateBuilder : CharacterMoveStateBuilder
    {
        public CreepMoveStateBuilder(CreepMoveState instance) : base(instance)
        {
        }


        public CreepMoveStateBuilder SetNavMesh(NavMeshAgent navMeshAgent)
        {
            if (state is CreepMoveState creepRunState)
                creepRunState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
        
        public CreepMoveStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is CreepMoveState creepRunState)
                creepRunState.SetRotationSpeed(rotationSpeed);
            return this;
        }
        
        public CreepMoveStateBuilder SetTimerRunToTarget(float value)
        {
            if (state is CreepMoveState creepRunState)
                creepRunState.SetTimerRunToTarget(value);
            return this;
        }
    }
}
