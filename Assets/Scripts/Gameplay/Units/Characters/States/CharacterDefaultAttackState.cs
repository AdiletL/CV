using System.Collections.Generic;
using Gameplay.Weapon;
using Movement;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterDefaultAttackState : CharacterBaseAttackState
    {
        protected CharacterSwitchMoveState characterSwitchMoveState;
        protected Rotation rotation;
        
        protected Collider[] findUnitColliders = new Collider[10];

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

        public override void Initialize()
        {
            base.Initialize();
            characterSwitchMoveState = this.StateMachine.GetState<CharacterSwitchMoveState>();
            rotation = new Rotation(GameObject.transform, characterSwitchMoveState.RotationSpeed);
        }

        public override void Enter()
        {
            base.Enter();

            FindUnit();
            
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
                this.StateMachine.ExitCategory(Category);
                return;
            }

            if(!Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, this.currentTarget.transform.position, this.range + .15f))
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }

            if (!Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, currentTarget.transform.position))
                rotation.Rotate();
               
            if(Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, currentTarget.transform.position, angleToTarget))
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

        protected void FindUnit()
        {
            currentTarget = Calculate.Attack.FindUnitInRange(Center.position, range, EnemyLayer, ref findUnitColliders);
            rotation.SetTarget(currentTarget.transform);
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
            FindUnit();
        }

        protected virtual void Cooldown()
        {
            if(isApplyDamage) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                if (currentTarget 
                    && Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, currentTarget.transform.position, angleToTarget))
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
    }
}