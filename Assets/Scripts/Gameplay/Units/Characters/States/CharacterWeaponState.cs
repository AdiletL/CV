using System.Collections.Generic;
using Gameplay.Weapon;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterWeaponState : CharacterBaseAttackState
    {
        protected CharacterSwitchMoveState characterSwitchMoveState;
        protected AnimationClip cooldownClip;
        protected List<AnimationClip> attackClips;

        protected float durationAttack, countDurationAttack;
        protected float applyDamage, countApplyDamage;
        protected float cooldown, countCooldown;
        protected float angleToTarget = 10;
        protected float range;

        protected bool isApplyDamage;
        protected bool isAttack;
        
        public SO_PlayerAttack SO_PlayerAttack { get; set; }
        public Weapon CurrentWeapon { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public Transform WeaponParent { get; set; }
        public Transform ProjectilePoint { get; set; }
        public int EnemyLayer { get; set; }
        

        protected AnimationClip getRandomAnimationClip()
        {
            return attackClips[Random.Range(0, attackClips.Count)];
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

            if(CurrentWeapon != null)
                FindTarget();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            CurrentWeapon.Show();
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
            CurrentWeapon?.Hide();
        }

        protected virtual void Restart()
        {
            isAttack = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countApplyDamage = 0;
            countCooldown = cooldown;
        }
        public virtual void SetWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
            durationAttack = Calculate.Attack.TotalDurationAttack(CurrentWeapon.AmountAttack);
            applyDamage = durationAttack * .55f;
            cooldown = durationAttack;
            range = CurrentWeapon.Range;

            switch (weapon)
            {
                case Sword sword:
                    SetAnimationClip(SO_PlayerAttack.SwordAttackClip, SO_PlayerAttack.SwordCooldownClip);
                    break;
                case Bow bow:
                    SetAnimationClip(SO_PlayerAttack.BowAttackClip, SO_PlayerAttack.BowCooldownClip);
                    break;
            }
            
            Restart();
        }

        public virtual void SetAnimationClip(List<AnimationClip> attackClips, AnimationClip cooldownClip)
        {
            this.attackClips = attackClips;
            this.cooldownClip = cooldownClip;
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
                    
                    //Debug.DrawRay(Center.position, direcitonToTarget * 100, Color.red, 2);
                    if (Physics.Raycast(Center.position, direcitonToTarget, out var hit2, 100) 
                        && hit2.transform.gameObject == hit.gameObject
                        && distanceToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceToTarget;
                        currentTarget = hit.transform.gameObject;
                        CurrentWeapon.SetTarget(currentTarget);
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
                    CurrentWeapon.ApplyDamage();
                else
                    currentTarget = null;
            }
            this.CharacterAnimation?.ChangeAnimation(cooldownClip);
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

    public class CharacterWeaponBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterWeaponBuilder(CharacterWeaponState instance) : base(instance)
        {
        }

        public CharacterWeaponBuilder SetGameObject(GameObject gameObject)
        {
            if (state is CharacterWeaponState characterWeapon)
            {
                characterWeapon.GameObject = gameObject;
            }

            return this;
        }
        
        public CharacterWeaponBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterWeaponState characterWeapon)
            {
                characterWeapon.CharacterAnimation = characterAnimation;
            }

            return this;
        }
        

        public CharacterWeaponBuilder SetEnemyLayer(int index)
        {
            if (state is CharacterWeaponState characterWeapon)
            {
                characterWeapon.EnemyLayer = index;
            }

            return this;
        }
        
        public CharacterWeaponBuilder SetCenter(Transform center)
        {
            if (state is CharacterWeaponState characterWeapon)
            {
                characterWeapon.Center = center;
            }

            return this;
        }

        public CharacterWeaponBuilder SetWeaponParent(Transform parent)
        {
            if (state is CharacterWeaponState characterWeapon)
            {
                characterWeapon.WeaponParent = parent;
            }

            return this;
        }

        public CharacterWeaponBuilder SetConfig(SO_PlayerAttack config)
        {
            if (state is CharacterWeaponState characterWeapon)
            {
                characterWeapon.SO_PlayerAttack = config;
            }

            return this;
        }
    }
}