using Gameplay.Weapon;
using Movement;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterWeaponAttackState : CharacterBaseAttackState
    {
        protected Rotation rotation;
        protected AnimationClip cooldownClip;
        protected AnimationClip[] attackClips;
        protected Transform weaponParent;
        protected UnitAnimation unitAnimation;
        protected CharacterSwitchMoveState characterSwitchMoveState;
        protected UnitEndurance unitEndurance;
        protected LayerMask enemyLayer;
        
        protected Collider[] findUnitColliders = new Collider[10];

        protected float baseReductionEndurance;
        protected float durationAttack, countDurationAttack;
        protected float cooldownApplyDamage, countTimerApplyDamage;
        protected float cooldown, countCooldown;
        protected float angleToTarget = 10;
        protected float rangeSqr;

        protected bool isApplyDamage;
        protected bool isAttack;

        public Weapon CurrentWeapon { get; protected set; }
        public float Range { get; protected set; }
        public float CurrentReductionEndurance { get; protected set; }
        
        public void SetWeaponParent(Transform parent) => weaponParent = parent;
        public void SetUnitAnimation(UnitAnimation animation) => unitAnimation = animation;
        public void SetCharacterSwitchMoveState(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMoveState = characterSwitchMoveState;
        public void SetUnitEndurance(UnitEndurance endurance) => unitEndurance = endurance;
        public void SetBaseReductionEndurance(float reductionEndurance) => baseReductionEndurance = reductionEndurance;
        public void SetEnemyLayer(LayerMask enemyLayer) => this.enemyLayer = enemyLayer;
        

        protected AnimationClip getRandomAnimationClip()
        {
            return attackClips[Random.Range(0, attackClips.Length)];
        }


        public override void Initialize()
        {
            base.Initialize();
            rotation = new Rotation(gameObject.transform, characterSwitchMoveState.RotationSpeed);
            UpdateDurationAttack();
            CurrentReductionEndurance = baseReductionEndurance;
        }

        public override void Enter()
        {
            base.Enter();
            /*
            if (!currentTarget)
            {
                this.stateMachine.ExitCategory(Category, null);
                return;
            }*/
            
            CurrentWeapon?.Show();
            unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
            ClearValues();
        }

        public override void Update()
        {
            base.Update();
            /*if (!currentTarget)
            {
                this.stateMachine.ExitCategory(Category, null);
                return;
            }

            if(!Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position, rangeSqr))
            {
                characterSwitchMoveState.SetTarget(currentTarget);
                characterSwitchMoveState.ExitCategory(Category);
                return;
            }

            if (!Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position, gameObject.transform.forward, currentTarget.transform.position))
                rotation.RotateToTarget();
               
            if(!isAttack && Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position, gameObject.transform.forward, currentTarget.transform.position, angleToTarget))
                isAttack = true;

            if (!isAttack)
            {
                unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
                return;
            }*/

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

        protected void UpdateDurationAttack()
        {
            durationAttack = Calculate.Attack.TotalDurationInSecond(AttackSpeed);
            cooldownApplyDamage = durationAttack * .55f;
            cooldown = durationAttack * .3f;
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
                CurrentWeapon.ResetCharacterStates(this);
            
            CurrentWeapon = weapon;
            Range = CurrentWeapon.Range;
            rangeSqr = Range * Range;
            CurrentWeapon.SetWeaponParent(weaponParent);

            CurrentWeapon.UpdateCharacterStates(this);
            
            ClearValues();
        }

        public virtual void RemoveWeapon()
        {
            CurrentWeapon.ResetCharacterStates(this);
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
            currentTarget = Calculate.Attack.FindUnitInRange(center.position, Range, enemyLayer, ref findUnitColliders);
            if(!currentTarget) return;
            
            CurrentWeapon.SetTarget(currentTarget);
            rotation.SetTarget(currentTarget.transform);
        }

        public override void Attack()
        {
            base.Attack();
            if (!isApplyDamage) return;
            
            countTimerApplyDamage += Time.deltaTime;
            if (countTimerApplyDamage > cooldownApplyDamage)
            {
                Fire();
                isApplyDamage = false;
                countTimerApplyDamage = 0;
            }
            unitEndurance.RemoveEndurance(CurrentReductionEndurance);
        }

        protected virtual void Fire()
        {
            FindUnit();
            if (currentTarget&& currentTarget.TryGetComponent(out IHealth health))
            {
                if (health.IsLive)
                    CurrentWeapon.FireAsync();
                else
                    currentTarget = null;
            }
            this.unitAnimation?.ChangeAnimationWithDuration(cooldownClip);
            isAttack = false;
            //FindUnit();
        }
        protected virtual void Cooldown()
        {
            if(isApplyDamage) return;
            countCooldown += Time.deltaTime;
            
            if (countCooldown > cooldown)
            {
                /*if (currentTarget)
                {
                    if (!currentTarget.TryGetComponent(out IHealth health) || !health.IsLive)
                    {
                        currentTarget = null;
                        return;
                    }
                    
                    this.unitAnimation?.ChangeAnimationWithDuration(getRandomAnimationClip(), duration: durationAttack);
                    isApplyDamage = true;
                }
                else
                {
                    isAttack = false;
                }*/
                this.unitAnimation?.ChangeAnimationWithDuration(getRandomAnimationClip(), duration: durationAttack);
                isApplyDamage = true;
                countCooldown = 0;
            }
            else
            {
                unitAnimation?.ChangeAnimationWithDuration(null, isDefault: true);
            }
        }

        public override void AddAttackSpeed(int amount)
        {
            base.AddAttackSpeed(amount);
            UpdateDurationAttack();
        }

        public override void RemoveAttackSpeed(int amount)
        {
            base.RemoveAttackSpeed(amount);
            UpdateDurationAttack();
        }

        public virtual void AddReductionEndurance(int amount)
        {
            CurrentReductionEndurance += amount;
        }

        public virtual void RemoveReductionEndurance(int amount)
        {
            CurrentReductionEndurance -= amount;
        }
    }

    public class CharacterWeaponAttackStateBuilder : CharacterBaseAttackStateBuilder
    {
        public CharacterWeaponAttackStateBuilder(CharacterWeaponAttackState instance) : base(instance)
        {
        }
        
        public CharacterWeaponAttackStateBuilder SetUnitAnimation(UnitAnimation unitAnimation)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetUnitAnimation(unitAnimation);

            return this;
        }

        public CharacterWeaponAttackStateBuilder SetWeaponParent(Transform parent)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetWeaponParent(parent);

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetCharacterSwitchMoveState(CharacterSwitchMoveState characterSwitchMoveState)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetCharacterSwitchMoveState(characterSwitchMoveState);

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetUnitEndurance(UnitEndurance endurance)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetUnitEndurance(endurance);

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetBaseReductionEndurance(float reductionEndurance)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetBaseReductionEndurance(reductionEndurance);

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetEnemyLayer(LayerMask enemyLayer)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetEnemyLayer(enemyLayer);

            return this;
        }
    }
}