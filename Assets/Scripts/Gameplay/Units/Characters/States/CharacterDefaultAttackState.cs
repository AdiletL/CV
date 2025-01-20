using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterDefaultAttackState : CharacterBaseAttackState
    {
        protected Rotation rotation;
        
        protected Collider[] findUnitColliders = new Collider[10];

        protected float durationAttack, countDurationAttack;
        protected float applyDamage, countApplyDamage;
        protected float cooldown, countCooldown;
        protected float angleToTarget = 10;
        protected float rangeSqr;

        protected bool isApplyDamage;
        protected bool isAttack;
        
        
        public CharacterSwitchMove CharacterSwitchMove { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public AnimationClip CooldownClip { get; set; }
        public AnimationClip[] AttackClips { get; set; }
        public LayerMask EnemyLayer { get; set; }
        public float Range { get; set; }
        
        protected AnimationClip getRandomAnimationClip()
        {
            return AttackClips[Random.Range(0, AttackClips.Length)];
        }

        public override void Initialize()
        {
            base.Initialize();
            rotation = new Rotation(GameObject.transform, CharacterSwitchMove.RotationSpeed);
            durationAttack = Calculate.Attack.TotalDurationInSecond(AttackSpeed);
            cooldown = durationAttack * .5f;
            applyDamage = durationAttack * .55f;
            rangeSqr = Range * Range;
        }

        public override void Enter()
        {
            base.Enter();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
            
            CharacterAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
            ResetValues();
        }

        public override void Update()
        {
            base.Update();
            
            if (!currentTarget
                || (!isAttack && !Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, this.currentTarget.transform.position, this.rangeSqr)))
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
            
            if (!isAttack)
            {
                if (Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, currentTarget.transform.position))
                    isAttack = true;
                else
                    rotation.Rotate();
                
                CharacterAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
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
            currentTarget = Calculate.Attack.FindUnitInRange(Center.position, Range, EnemyLayer, ref findUnitColliders);
            rotation.SetTarget(currentTarget?.transform);
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
                this.CharacterAnimation?.ChangeAnimationWithDuration(getRandomAnimationClip(), duration: durationAttack);
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
                Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, this.currentTarget.transform.position, this.rangeSqr) &&
                Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position,
                    this.GameObject.transform.forward, currentTarget.transform.position, angleToTarget))
            {
                if (health.IsLive)
                    attackable.TakeDamage(Damageable);
                else
                    currentTarget = null;
            }
            this.CharacterAnimation?.ChangeAnimationWithDuration(CooldownClip);
            isAttack = false;
            FindUnit();
        }
    }
    
    
    public class CharacterDefaultAttackStateBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterDefaultAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
        }
        
        public CharacterDefaultAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.GameObject = gameObject;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.CharacterAnimation = characterAnimation;
            }

            return this;
        }
        

        public CharacterDefaultAttackStateBuilder SetEnemyLayer(LayerMask layer)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.EnemyLayer = layer;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetCenter(Transform center)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.Center = center;
            }

            return this;
        }

        public CharacterDefaultAttackStateBuilder SetAttackClips(AnimationClip[] animationClip)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.AttackClips = animationClip;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetCooldownClip(AnimationClip animationClip)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.CooldownClip = animationClip;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetRange(float range)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.Range = range;
            }

            return this;
        }
        public CharacterDefaultAttackStateBuilder SetCharacterSwitchMove(CharacterSwitchMove characterSwitchMove)
        {
            if (state is CharacterDefaultAttackState characterDefaultAttackState)
            {
                characterDefaultAttackState.CharacterSwitchMove = characterSwitchMove;
            }

            return this;
        }
    }
}