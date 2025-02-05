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
        protected float applyDamage, countApplyDamage;
        protected float cooldown, countCooldown;
        protected float angleToTarget = 10;
        protected float rangeSqr;
        
        protected bool isApplyDamage;
        protected bool isAttack;

        public float Range { get; protected set; }
        
        public void SetSwitchMoveState(CharacterSwitchMoveState characterSwitchState) => this.characterSwitchMoveState = characterSwitchState;
        public void SetUnitAnimation(UnitAnimation unitAnimation) => this.unitAnimation = unitAnimation;
        public void SetCooldownClip(AnimationClip cooldownClip) => this.cooldownClip = cooldownClip;
        public void SetAttackClips(AnimationClip[] attackClips) => this.attackClips = attackClips;
        public void SetEnemyLayer(LayerMask enemyLayer) => this.enemyLayer = enemyLayer;
        public void SetRange(float range) => this.Range = range;
        
        
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
            applyDamage = durationAttack * .55f;
            rangeSqr = Range * Range;
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
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
            
            if (!isAttack)
            {
                if (Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position, gameObject.transform.forward, currentTarget.transform.position))
                    isAttack = true;
                else
                    RotateToTarget();
                
                unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
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

        protected virtual void ResetValues()
        {
            isAttack = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countApplyDamage = 0;
            countCooldown = 0;
        }

        protected void FindUnit()
        {
            currentTarget = Calculate.Attack.FindUnitInRange(center.position, Range, enemyLayer, ref findUnitColliders);
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
        }
        
        public override void Attack()
        {
            base.Attack();
            if (!isApplyDamage) return;
            
            countApplyDamage += Time.deltaTime;
            if (countApplyDamage > applyDamage)
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
                Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position,
                    gameObject.transform.forward, currentTarget.transform.position, angleToTarget))
            {
                if (health.IsLive)
                    attackable.TakeDamage(Damageable);
                else
                    currentTarget = null;
            }
            unitAnimation?.ChangeAnimationWithDuration(cooldownClip);
            isAttack = false;
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
        public CharacterDefaultAttackStateBuilder SetRange(float range)
        {
            if(state is CharacterDefaultAttackState defaultState)
                defaultState.SetRange(range);
            return this;
        }
    }
}