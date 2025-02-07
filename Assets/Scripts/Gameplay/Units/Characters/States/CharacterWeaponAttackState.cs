using Gameplay.Weapon;
using Movement;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterWeaponAttackState : CharacterBaseAttackState
    {
        protected Transform weaponParent;
        protected UnitAnimation unitAnimation;
        protected UnitEndurance unitEndurance;
        protected AnimationClip[] attackClips;
        protected LayerMask enemyLayer;
        
        protected Collider[] findUnitColliders = new Collider[10];

        protected float baseReductionEndurance;
        protected float durationAttack, countDurationAttack;
        protected float cooldownApplyDamage, countTimerApplyDamage;
        protected float angleToTarget = 100;
        protected float rangeSqr;
        
        public Weapon CurrentWeapon { get; protected set; }
        public float Range { get; protected set; }
        public float CurrentReductionEndurance { get; protected set; }
        
        public void SetWeaponParent(Transform parent) => weaponParent = parent;
        public void SetUnitAnimation(UnitAnimation animation) => unitAnimation = animation;
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
            UpdateDurationAttack();
            CurrentReductionEndurance = baseReductionEndurance;
        }

        public override void Enter()
        {
            base.Enter();

            CurrentWeapon?.Show();
            ClearValues();
        }

        public override void Update()
        {
            base.Update();
            RotateToTarget();
            Attack();
            CheckDurationAttack();
        }
        
        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
            CurrentWeapon?.Hide();
        }

        protected virtual void ClearValues()
        {
            countDurationAttack = 0;
            countTimerApplyDamage = 0;
        }

        protected void UpdateDurationAttack()
        {
            durationAttack = Calculate.Attack.TotalDurationInSecond(AttackSpeed);
            cooldownApplyDamage = durationAttack * .55f;
        }
        
        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            CurrentWeapon?.SetTarget(currentTarget);
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
            SetAnimationClip(null);
        }

        protected virtual void SetAnimationClip(AnimationClip[] attackClips)
        {
            this.attackClips = attackClips;
        }

        protected void FindUnit()
        {
            currentTarget = Calculate.Attack.FindUnitInRange(center.position, Range, enemyLayer, ref findUnitColliders);
            if (!currentTarget)
            {
                stateMachine.ExitCategory(Category, null);
                return;
            }
            
            CurrentWeapon.SetTarget(currentTarget);
        }

        protected virtual void CheckDurationAttack()
        {
            countDurationAttack += Time.deltaTime;
            if (countDurationAttack > durationAttack)
            {
                stateMachine.ExitCategory(Category, null);
                countDurationAttack = 0;
            }
        }
        
        protected virtual void RotateToTarget()
        {
            
        }
        
        public override void Attack()
        {
            base.Attack();
            
            countTimerApplyDamage += Time.deltaTime;
            if (countTimerApplyDamage > cooldownApplyDamage)
            {
                Fire();
                countTimerApplyDamage = 0;
            }
            unitEndurance.RemoveEndurance(CurrentReductionEndurance);
        }

        protected virtual void Fire()
        {
           
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