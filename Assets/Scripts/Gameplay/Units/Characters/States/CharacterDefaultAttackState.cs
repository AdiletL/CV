using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterDefaultAttackState : CharacterBaseAttackState
    {
        protected Rotation rotation;
        protected UnitAnimation unitAnimation;
        protected CharacterSwitchMoveState characterSwitchMoveState;
        protected AnimationClip cooldownClip;
        protected AnimationClip[] attackClips;
        protected LayerMask enemyLayer;
        
        protected Collider[] findUnitColliders = new Collider[10];
        
        protected float durationAttack, countDurationAttack;
        protected float cooldownApplyDamage, countApplyDamage;
        protected float cooldown, countCooldown;
        protected float applyDamageMoment;
        protected float angleToTarget = 10;
        protected float rangeSqr;
        
        protected bool isApplyDamage;
        protected bool isCooldown;

        public Stat RangeStat { get; private set; } = new();

        public void SetSwitchMoveState(CharacterSwitchMoveState characterSwitchState) => this.characterSwitchMoveState = characterSwitchState;
        public void SetUnitAnimation(UnitAnimation unitAnimation) => this.unitAnimation = unitAnimation;
        public void SetCooldownClip(AnimationClip cooldownClip) => this.cooldownClip = cooldownClip;
        public void SetAttackClips(AnimationClip[] attackClips) => this.attackClips = attackClips;
        public void SetEnemyLayer(LayerMask enemyLayer) => this.enemyLayer = enemyLayer;
        public void SetApplyDamageMoment(float applyDamageMoment) => this.applyDamageMoment = applyDamageMoment;
        
        
        protected AnimationClip getRandomAnimationClip()
        {
            return attackClips[Random.Range(0, attackClips.Length)];
        }

        public override void Initialize()
        {
            base.Initialize();
            rotation = new Rotation(gameObject.transform, characterSwitchMoveState.RotationSpeed);
            durationAttack = Calculate.Attack.TotalDurationInSecond(AttackSpeed);
            cooldown = durationAttack * .5f;
            cooldownApplyDamage = durationAttack * applyDamageMoment;
            rangeSqr = RangeStat.CurrentValue * RangeStat.CurrentValue;
        }

        public override void Enter()
        {
            base.Enter();
            
            unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
            ResetValues();
        }

        public override void Update()
        {
            base.Update();
            
            if (!currentTarget)
            {
                this.stateMachine.ExitCategory(Category, null);
                return;
            }

            if (!isCooldown && !Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                    gameObject.transform.forward, currentTarget.transform.position, angleToTarget))
            {
                RotateToTarget();
                return;
            }
            
            Cooldown();
            Attack();
        }
        
        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            if(target == null) return;
            rotation.SetTarget(target.transform);
        }

        protected virtual void ResetValues()
        {
            isCooldown = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countApplyDamage = 0;
            countCooldown = 0;
        }

        protected virtual void FindUnit()
        {
            currentTarget = Calculate.Attack.FindUnitInRange(center.position, RangeStat.CurrentValue, enemyLayer, ref findUnitColliders);
            rotation.SetTarget(currentTarget?.transform);
        }

        protected virtual void RotateToTarget()
        {
            rotation.RotateToTarget();
        }
        
        protected virtual void Cooldown()
        {
            if(isApplyDamage) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                if (!currentTarget.TryGetComponent(out IHealth health) || !health.IsLive)
                {
                    currentTarget = null;
                    return;
                }
                unitAnimation?.ChangeAnimationWithDuration(getRandomAnimationClip(), duration: durationAttack);
                isApplyDamage = true;
                countCooldown = 0;
            }
            else
            {
                unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
            }

            isCooldown = true;
        }
        
        public override void Attack()
        {
            base.Attack();
            if (!isApplyDamage) return;
            
            countApplyDamage += Time.deltaTime;
            if (countApplyDamage > cooldownApplyDamage)
            {
                ApplyDamage();
                isApplyDamage = false;
                countApplyDamage = 0;
            }
            
        }
        
        public override void ApplyDamage()
        {
            if (currentTarget &&
                currentTarget.TryGetComponent(out IAttackable attackable) &&
                currentTarget.TryGetComponent(out IHealth health) &&
                Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, this.currentTarget.transform.position, this.rangeSqr) &&
                Calculate.Rotate.IsFacingTargetUsingAngle(gameObject.transform.position,
                    gameObject.transform.forward, currentTarget.transform.position, angleToTarget))
            {
                if (health.IsLive)
                    attackable.TakeDamage(Damageable);
                else
                    currentTarget = null;
            }
            unitAnimation?.ChangeAnimationWithDuration(cooldownClip);
            isCooldown = false;
        }
    }
    
    
    public class CharacterDefaultAttackStateBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterDefaultAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
        }

        public CharacterDefaultAttackStateBuilder SetSwitchMoveState(CharacterSwitchMoveState characterSwitchState)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetSwitchMoveState(characterSwitchState);
            return this;
        }

        public CharacterDefaultAttackStateBuilder SetUnitAnimation(UnitAnimation unitAnimation)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetUnitAnimation(unitAnimation);
            return this;
        }
        public CharacterDefaultAttackStateBuilder SetCooldownClip(AnimationClip cooldownClip)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetCooldownClip(cooldownClip);
            return this;
        }
        public CharacterDefaultAttackStateBuilder SetAttackClips(AnimationClip[] attackClips)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetAttackClips(attackClips);
            return this;
        }
        public CharacterDefaultAttackStateBuilder SetEnemyLayer(LayerMask enemyLayer)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetEnemyLayer(enemyLayer);
            return this;
        }
        public CharacterDefaultAttackStateBuilder SetApplyDamageMoment(float applyDamageMoment)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetApplyDamageMoment(applyDamageMoment);
            return this;
        }
    }
}