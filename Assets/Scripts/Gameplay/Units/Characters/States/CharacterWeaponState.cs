using System.Collections.Generic;
using Gameplay.Weapon;
using Movement;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterWeaponState : CharacterBaseAttackState
    {
        protected CharacterSwitchMoveState characterSwitchMoveState;
        protected Rotation rotation;
        protected AnimationClip cooldownClip;
        protected List<AnimationClip> attackClips;
        
        protected Collider[] findUnitColliders = new Collider[10];

        protected float durationAttack, countDurationAttack;
        protected float timerApplyDamage, countTimerApplyDamage;
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
        public int EnemyLayer { get; set; }
        

        protected AnimationClip getRandomAnimationClip()
        {
            return attackClips[Random.Range(0, attackClips.Count)];
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

            if (CurrentWeapon != null)
            {
                FindUnit();
            }
            
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
                this.StateMachine.ExitCategory(Category);
                return;
            }

            if(!Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position, (range * range) + .15f))
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
            CurrentWeapon?.Hide();
        }

        protected virtual void Restart()
        {
            isAttack = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countTimerApplyDamage = 0;
            countCooldown = cooldown;
        }
        public virtual void SetWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
            durationAttack = Calculate.Attack.TotalDurationAttack(CurrentWeapon.AmountAttack);
            timerApplyDamage = durationAttack * .55f;
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

        private void FindUnit()
        {
            currentTarget = Calculate.Attack.FindUnitInRange(Center.position, range, EnemyLayer, ref findUnitColliders);
            CurrentWeapon.SetTarget(currentTarget);
            rotation.SetTarget(currentTarget?.transform);
        }
        

        public override void Attack()
        {
            base.Attack();
            if (!isApplyDamage) return;
            
            countTimerApplyDamage += Time.deltaTime;
            if (countTimerApplyDamage > timerApplyDamage)
            {
                Fire();
                isApplyDamage = false;
                countTimerApplyDamage = 0;
            }
            
        }

        protected virtual void Fire()
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
            FindUnit();
        }
        public override void ApplyDamage()
        {
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