using System.Collections.Generic;
using Gameplay.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterDefaultAttackState : CharacterBaseAttackState
    {
        protected CharacterSwitchMoveState characterSwitchMoveState;

        protected float durationAttack, countDurationAttack;
        protected float applyDamage, countApplyDamage;
        protected float cooldown, countCooldown;
        protected float angleToTarget = 10;
        protected float range;

        protected bool isApplyDamage;
        protected bool isAttack;
        
        public CharacterAnimation CharacterAnimation { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public AnimationClip CooldownClip { get; set; }
        public List<AnimationClip> AttackClips { get; set; }
        public int EnemyLayer { get; set; }
        

        protected AnimationClip getRandomAnimationClip()
        {
            return AttackClips[Random.Range(0, AttackClips.Count)];
        }
        
        protected bool isCheckDistanceToTarget()
        {
            float distanceToTarget = Vector3.Distance(this.GameObject.transform.position, this.currentTarget.transform.position);
            if (distanceToTarget > range + .15f)
            {
                return false;
            }

            return true;
        }


        public override void Initialize()
        {
            base.Initialize();
            characterSwitchMoveState = this.StateMachine.GetState<CharacterSwitchMoveState>();
        }

        public override void Enter()
        {
            base.Enter();

            FindTarget();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            CharacterAnimation?.ChangeAnimation(null, isDefault: true);
            Restart();
        }

        public override void Update()
        {
            base.Update();

            if (!currentTarget)
            {
                FindTarget();
                if (!currentTarget)
                {
                    this.StateMachine.ExitCategory(Category);
                    return;
                }
            }

            if(!isCheckDistanceToTarget())
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }

            if (!Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform, currentTarget.transform))
                Calculate.Move.Rotate(this.GameObject.transform, currentTarget.transform, characterSwitchMoveState.RotationSpeed, ignoreX: true, ignoreY: false, ignoreZ: true);
               
            if(Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform, currentTarget.transform, angleToTarget))
                isAttack = true;

            if (!isAttack)
            {
                CharacterAnimation?.ChangeAnimation(null, isDefault: true);
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

        protected virtual void Restart()
        {
            isAttack = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countApplyDamage = 0;
            countCooldown = cooldown;
        }
        
        protected void FindTarget()
        {
            Collider[] hits = Physics.OverlapSphere(Center.position, range, EnemyLayer);
            float closestDistanceSqr = range;

            foreach (var hit in hits)
            {
                if(!hit.TryGetComponent(out IHealth health) || !health.IsLive) continue;
                
                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    float distanceToTarget = Vector3.Distance(this.GameObject.transform.position, hit.transform.position);
                    var direcitonToTarget = (unitCenter.Center.position - Center.position).normalized;
                    
                    if (Physics.Raycast(Center.position, direcitonToTarget, out var hit2, range) 
                        && hit2.transform.gameObject == hit.gameObject
                        && distanceToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceToTarget;
                        currentTarget = hit.transform.gameObject;
                    }
                }
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
            if (currentTarget&& currentTarget.TryGetComponent(out IHealth health))
            {
                if (health.IsLive)
                    health.TakeDamage(Damageable);
                else
                    currentTarget = null;
            }
            this.CharacterAnimation?.ChangeAnimation(CooldownClip);
            isAttack = false;
        }

        protected virtual void Cooldown()
        {
            if(isApplyDamage) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                if (currentTarget 
                    && Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform, currentTarget.transform, angleToTarget))
                {
                    if (!currentTarget.TryGetComponent(out IHealth health) || !health.IsLive)
                    {
                        currentTarget = null;
                        return;
                    }
                    
                    this.CharacterAnimation?.ChangeAnimation(getRandomAnimationClip(), duration: durationAttack);
                    isApplyDamage = true;
                }
                else
                {
                    isAttack = false;
                }

                countCooldown = 0;
            }
        }
    }
    
    
    public class CharacterDefaultAttackStateBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterDefaultAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
        }
        
        public CharacterDefaultAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.GameObject = gameObject;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.CharacterAnimation = characterAnimation;
            }

            return this;
        }
        

        public CharacterDefaultAttackStateBuilder SetEnemyLayer(int index)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.EnemyLayer = index;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetCenter(Transform center)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.Center = center;
            }

            return this;
        }

        public CharacterDefaultAttackStateBuilder SetAttackClips(List<AnimationClip> animationClip)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.AttackClips = animationClip;
            }

            return this;
        }
        
        public CharacterDefaultAttackStateBuilder SetCooldownClip(AnimationClip animationClip)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.CooldownClip = animationClip;
            }

            return this;
        }
        public CharacterDefaultAttackStateBuilder SetAmountAttack(int amount)
        {
            if (state is CharacterDefaultAttackState characterWeapon)
            {
                characterWeapon.AmountAttack = amount;
            }

            return this;
        }
    }
}