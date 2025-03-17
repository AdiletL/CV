using Movement;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Unit.Character.Creep
{
    public class CreepAttackState : CharacterAttackState
    {
        protected NavMeshAgent navMeshAgent;
        protected Rotation rotation;
        protected float rangeSqr;
        protected const float timerExitState = 7;
        protected float countTimerExitState;
        protected bool isFacingTarget;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;

        public override bool IsUnitInRange()
        {
            currentTarget = FindUnitInRange<ICreepInteractable>();
            return currentTarget;
        }

        public override void Initialize()
        {
            base.Initialize();
            rangeSqr = RangeStat.CurrentValue * RangeStat.CurrentValue;
            rotation = new Rotation(gameObject.transform, so_CharacterAttack.RotationSpeed);
        }

        public override void Enter()
        {
            base.Enter();
            navMeshAgent.updateRotation = false;
            UpdateCurrentClip();
        }

        public override void Update()
        {
            if (!currentTarget)
            {
                if (!IsUnitInRange())
                {
                    stateMachine.ExitCategory(Category, null);
                    return;
                }
            }

            CheckNearTarget();
            CheckFacingOnTarget();
            if (isFacingTarget)
            {
                CheckDurationAttack();
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();
            navMeshAgent.updateRotation = true;
        }

        protected override void ClearValues()
        {
            base.ClearValues();
            isFacingTarget = false;
            countTimerExitState = 0;
        }

        private void CheckDurationAttack()
        {
            countDurationAttack += Time.deltaTime;
            if (durationAttack < countDurationAttack)
            {
                if (!currentTarget ||
                    !currentTarget.TryGetComponent(out IHealth health) ||
                    !health.IsLive)
                {
                    currentTarget = null;
                }
                else
                {
                    this.unitAnimation.ChangeAnimationWithDuration(null, isDefault: true);
                    UpdateCurrentClip();
                }
                ClearValues();
                countDurationAttack = 0;
            }
        }
        
        private void CheckNearTarget()
        {
            if (currentTarget && 
                !Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position,
                    rangeSqr) || isObstacleBetween(currentTarget))
            {
                var target = FindUnitInRange<ICreepInteractable>();
                if (!target)
                {
                    stateMachine.GetState<CreepMoveState>().SetTarget(currentTarget);
                    stateMachine.ExitCategory(Category, typeof(CreepMoveState));
                }
            }
        }
        private void CheckFacingOnTarget()
        {
            if(!currentTarget) return;
            if (!isFacingTarget)
            {
                if (Calculate.Rotate.IsFacingTargetXZ(gameObject.transform.position, gameObject.transform.forward, currentTarget.transform.position) && 
                    Calculate.Rotate.IsFacingTargetY(gameObject.transform, currentTarget.transform.position, 50))
                {
                    countTimerExitState = 0;
                    isFacingTarget = true;
                }
                else
                {
                    this.unitAnimation.ChangeAnimationWithDuration(null, isDefault: true);
                    rotation.SetTarget(currentTarget.transform);
                    rotation.RotateToTarget();
                    TimerExitState();
                }
            }
        }

        private void TimerExitState()
        {
            countTimerExitState += Time.deltaTime;
            if (timerExitState < countTimerExitState)
                stateMachine.ExitCategory(Category, null);
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