using Gameplay.Weapon;
using Movement;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterWeaponAttackState : CharacterBaseAttackState
    {
        protected Rotation rotation;
        protected AnimationClip cooldownClip;
        protected AnimationClip[] attackClips;
        
        protected Collider[] findUnitColliders = new Collider[10];

        protected float durationAttack, countDurationAttack;
        protected float timerApplyDamage, countTimerApplyDamage;
        protected float cooldown, countCooldown;
        protected float angleToTarget = 10;
        protected float rangeSqr;
        protected float attackDecreaseEndurance;

        protected bool isApplyDamage;
        protected bool isAttack;
        
        
        public CharacterSwitchMove CharacterSwitchMove { get; set; }
        public CharacterEndurance CharacterEndurance { get; set; }
        public SO_PlayerAttack SO_PlayerAttack { get; set; }
        public Weapon CurrentWeapon { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public Transform WeaponParent { get; set; }
        public int EnemyLayer { get; set; }
        public float Range { get; protected set; }
        

        protected AnimationClip getRandomAnimationClip()
        {
            return attackClips[Random.Range(0, attackClips.Length)];
        }


        public override void Initialize()
        {
            base.Initialize();
            rotation = new Rotation(GameObject.transform, CharacterSwitchMove.RotationSpeed);
            durationAttack = Calculate.Attack.TotalDurationInSecond(AttackSpeed);
            timerApplyDamage = durationAttack * .55f;
            cooldown = durationAttack * .5f;
        }

        public override void Enter()
        {
            base.Enter();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
            
            CurrentWeapon.Show();
            CharacterAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
            ClearValues();
        }

        public override void Update()
        {
            base.Update();
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }

            if(!Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position, rangeSqr))
            {
                CharacterSwitchMove.SetTarget(currentTarget);
                CharacterSwitchMove.ExitCategory(Category);
                return;
            }

            if (!Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, currentTarget.transform.position))
                rotation.Rotate();
               
            if(!isAttack && Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, currentTarget.transform.position, angleToTarget))
                isAttack = true;

            if (!isAttack)
            {
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
            CurrentWeapon?.Hide();
        }

        protected virtual void ClearValues()
        {
            isAttack = false;
            isApplyDamage = false;
            countDurationAttack = 0;
            countTimerApplyDamage = 0;
            countCooldown = 0;
        }

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            CurrentWeapon?.SetTarget(currentTarget);
            rotation.SetTarget(currentTarget.transform);
        }

        public virtual void SetWeapon(Weapon weapon)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Damageable.RemoveAdditionalDamage(Damageable.CurrentDamage);
                IncreaseAttackSpeed(CurrentWeapon.DecreaseAttackSpeed);
            }
            
            CurrentWeapon = weapon;
            Range = CurrentWeapon.Range;
            rangeSqr = Range * Range;
            attackDecreaseEndurance = weapon.DecreaseEndurance;
            DecreaseAttackSpeed(CurrentWeapon.DecreaseAttackSpeed);

            CurrentWeapon.Damageable.AddAdditionalDamage(Damageable.CurrentDamage);

            switch (weapon)
            {
                case Sword sword:
                    SetAnimationClip(SO_PlayerAttack.SwordAttackClip, SO_PlayerAttack.SwordCooldownClip);
                    break;
                case Bow bow:
                    SetAnimationClip(SO_PlayerAttack.BowAttackClip, SO_PlayerAttack.BowCooldownClip);
                    break;
            }
            
            ClearValues();
        }

        public virtual void RemoveWeapon()
        {
            CurrentWeapon?.Damageable.RemoveAdditionalDamage(Damageable.CurrentDamage);
            IncreaseAttackSpeed(CurrentWeapon.DecreaseAttackSpeed);
            CurrentWeapon = null;
            SetAnimationClip(null, null);
        }

        protected virtual void SetAnimationClip(AnimationClip[] attackClips, AnimationClip cooldownClip)
        {
            this.attackClips = attackClips;
            this.cooldownClip = cooldownClip;
        }

        private void FindUnit()
        {
            currentTarget = Calculate.Attack.FindUnitInRange(Center.position, Range, EnemyLayer, ref findUnitColliders);
            if(!currentTarget) return;
            
            CurrentWeapon.SetTarget(currentTarget);
            rotation.SetTarget(currentTarget.transform);
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
            CharacterEndurance.RemoveEndurance(attackDecreaseEndurance);
        }

        protected virtual void Fire()
        {
            if (currentTarget&& currentTarget.TryGetComponent(out IHealth health))
            {
                if (health.IsLive)
                    CurrentWeapon.FireAsync();
                else
                    currentTarget = null;
            }
            this.CharacterAnimation?.ChangeAnimationWithDuration(cooldownClip);
            isAttack = false;
            //FindUnit();
        }
        protected virtual void Cooldown()
        {
            if(isApplyDamage) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                if (currentTarget)
                {
                    if (!currentTarget.TryGetComponent(out IHealth health) || !health.IsLive)
                    {
                        currentTarget = null;
                        return;
                    }
                    
                    this.CharacterAnimation?.ChangeAnimationWithDuration(getRandomAnimationClip(), duration: durationAttack);
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

    public class CharacterWeaponAttackStateBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterWeaponAttackStateBuilder(CharacterWeaponAttackState instance) : base(instance)
        {
        }

        public CharacterWeaponAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.GameObject = gameObject;
            }

            return this;
        }
        
        public CharacterWeaponAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.CharacterAnimation = characterAnimation;
            }

            return this;
        }
        

        public CharacterWeaponAttackStateBuilder SetEnemyLayer(int index)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.EnemyLayer = index;
            }

            return this;
        }
        
        public CharacterWeaponAttackStateBuilder SetCenter(Transform center)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.Center = center;
            }

            return this;
        }

        public CharacterWeaponAttackStateBuilder SetWeaponParent(Transform parent)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.WeaponParent = parent;
            }

            return this;
        }

        public CharacterWeaponAttackStateBuilder SetConfig(SO_PlayerAttack config)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.SO_PlayerAttack = config;
            }

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetCharacterEndurance(CharacterEndurance endurance)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.CharacterEndurance = endurance;
            }

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetCharacterSwitchMove(CharacterSwitchMove characterSwitchMove)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
            {
                characterWeapon.CharacterSwitchMove = characterSwitchMove;
            }

            return this;
        }
    }
}