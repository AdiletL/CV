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
        protected bool isFacingTarget;
        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;

        public override bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<ICreepAttackable>(center.position, RangeStat.CurrentValue, enemyLayer, ref findUnitColliders);
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
        }

        public override void Update()
        {
            if (!currentTarget)
            {
                FindUnitInRange();
                if (!currentTarget)
                {
                    stateMachine.ExitCategory(Category, null);
                    return;
                }
            }
            
            CheckNearTarget();
            CheckFacingTarget();
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
                    rangeSqr))
            {
                FindUnitInRange();
                stateMachine.GetState<CreepMoveState>().SetTarget(currentTarget);
                stateMachine.ExitCategory(Category, typeof(CreepMoveState));
            }
        }
        private void CheckFacingTarget()
        {
            if(!currentTarget) return;
            if (!isFacingTarget)
            {
                if (Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                    gameObject.transform.forward, currentTarget.transform.position, 10))
                {
                    this.unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
                    isFacingTarget = true;
                }
                else
                {
                    rotation.SetTarget(currentTarget.transform);
                    rotation.RotateToTarget();
                }
            }
        }

        protected override void DefaultApplyDamage()
        {
            if(currentTarget &&
               Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                   gameObject.transform.forward, currentTarget.transform.position, angleToTarget) &&
               currentTarget.TryGetComponent(out IAttackable attackable) && 
               currentTarget.TryGetComponent(out IHealth health) && health.IsLive)
            {
                Damageable.Value = DamageStat.CurrentValue;
                attackable.TakeDamage(Damageable);
            }
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