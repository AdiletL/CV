using System;
using System.Collections.Generic;
using Gameplay.Equipment.Weapon;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unit.Character
{
    public class CharacterWeaponAttackState : State
    {
        public override StateCategory Category { get; } = StateCategory.Attack;

        protected SO_CharacterAttack so_CharacterAttack;
        protected GameObject gameObject;
        protected GameObject currentTarget;
        protected Transform weaponParent;
        protected Transform center;
        protected UnitAnimation unitAnimation;
        protected UnitEndurance unitEndurance;
        protected UnitRenderer unitRenderer;
        protected AnimationClip[] attackClips;
        protected LayerMask enemyLayer;
        
        protected float durationAttack, countDurationAttack;
        protected float cooldownApplyDamage, countTimerApplyDamage;
        protected float applyDamageMoment;
        
        protected bool isAttacked;
        
        public Weapon CurrentWeapon { get; protected set; }
        public Stat AttackSpeedStat { get; } = new();
        public Stat DamageStat { get; } = new();
        public Stat ReduceEnduranceStat { get; } = new();
        public Stat RangeStat { get; } = new();


        ~CharacterWeaponAttackState()
        {
            UnsubscribeEvent();
        }

        public void SetConfig(SO_CharacterAttack config) => so_CharacterAttack = config;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        public void SetWeaponParent(Transform parent) => weaponParent = parent;
        public void SetUnitAnimation(UnitAnimation animation) => unitAnimation = animation;
        public void SetUnitEndurance(UnitEndurance endurance) => unitEndurance = endurance;
        public void SetUnitRenderer(UnitRenderer unitRenderer) => this.unitRenderer = unitRenderer;
        

        protected AnimationClip getRandomAnimationClip()
        {
            return attackClips[Random.Range(0, attackClips.Length)];
        }

        public override void Initialize()
        {
            base.Initialize();
            SubscribeEvent();
            
            enemyLayer = so_CharacterAttack.EnemyLayer;
            applyDamageMoment = so_CharacterAttack.ApplyDamageMoment;
            unitRenderer.SetRangeScale(RangeStat.CurrentValue);
            unitRenderer.ShowRangeVisual();
        }

        public override void Enter()
        {
            base.Enter();

            CurrentWeapon?.Show();
            ClearValues();
        }

        public override void Update()
        {

        }

        public override void LateUpdate()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }

        protected virtual void SubscribeEvent()
        {
            DamageStat.OnAddCurrentValue += OnAddDamageStatCurrentValue;
            DamageStat.OnRemoveCurrentValue += OnRemoveDamageStatCurrentValue;
            RangeStat.OnAddCurrentValue += OnAddRangeStatCurrentValue;
            RangeStat.OnRemoveCurrentValue += OnRemoveRangeStatCurrentValue;
            AttackSpeedStat.OnAddCurrentValue += OnAddAttackSpeedStatCurrentValue;
            AttackSpeedStat.OnRemoveCurrentValue += OnRemoveAttackSpeedStatCurrentValue;
        }

        protected virtual void UnsubscribeEvent()
        {
            DamageStat.OnAddCurrentValue -= OnAddDamageStatCurrentValue;
            DamageStat.OnRemoveCurrentValue -= OnRemoveDamageStatCurrentValue;
            RangeStat.OnAddCurrentValue -= OnAddRangeStatCurrentValue;
            RangeStat.OnRemoveCurrentValue -= OnRemoveRangeStatCurrentValue;
            AttackSpeedStat.OnAddCurrentValue -= OnAddAttackSpeedStatCurrentValue;
            AttackSpeedStat.OnRemoveCurrentValue -= OnRemoveAttackSpeedStatCurrentValue;
        }
        private void OnAddDamageStatCurrentValue(float value) => CurrentWeapon?.DamageStat.AddValue(value);
        private void OnRemoveDamageStatCurrentValue(float value) => CurrentWeapon?.DamageStat.RemoveValue(value);
        private void OnAddRangeStatCurrentValue(float value)
        {
            if(CurrentWeapon == null) return;
            CurrentWeapon.RangeStat.AddValue(value);
            unitRenderer.SetRangeScale(CurrentWeapon.RangeStat.CurrentValue);
        }
        private void OnRemoveRangeStatCurrentValue(float value)
        {
            CurrentWeapon.RangeStat.RemoveValue(value);
            unitRenderer.SetRangeScale(CurrentWeapon.RangeStat.CurrentValue);
        }

        private void OnAddAttackSpeedStatCurrentValue(float value) => UpdateDurationAttack();
        private void OnRemoveAttackSpeedStatCurrentValue(float value) => UpdateDurationAttack();
        

        public void SetTarget(GameObject target) => currentTarget = target;
        
        protected virtual void ClearValues()
        {
            countDurationAttack = 0;
            countTimerApplyDamage = 0;
            isAttacked = false;
        }

        protected void UpdateDurationAttack()
        {
            durationAttack = Calculate.Attack.TotalDurationInSecond(AttackSpeedStat.CurrentValue);
            cooldownApplyDamage = durationAttack * applyDamageMoment;
        }
        
        public virtual void SetWeapon(Weapon weapon)
        {
            if(weapon == CurrentWeapon) return;
            RemoveWeapon();
            
            CurrentWeapon = weapon;
            CurrentWeapon.SetInParent(weaponParent);
            CurrentWeapon.SetEnemyLayer(enemyLayer);
            CurrentWeapon.Show();
            ClearValues();
            UpdateDurationAttack();
            CurrentWeapon.DamageStat.AddValue(DamageStat.CurrentValue);
            ReduceEnduranceStat.AddValue(CurrentWeapon.ReduceEndurance);
        }

        public void RemoveWeapon()
        {
            if(CurrentWeapon == null) return;
            ReduceEnduranceStat.RemoveValue(CurrentWeapon.ReduceEndurance);
            CurrentWeapon.DamageStat.RemoveValue(DamageStat.CurrentValue);
            CurrentWeapon.Hide();
            CurrentWeapon = null;
            SetAnimationClip(null);
        }

        protected virtual void SetAnimationClip(AnimationClip[] attackClips)
        {
            this.attackClips = attackClips;
        }
        
        protected void Attack()
        {
            if(isAttacked) return;
            
            countTimerApplyDamage += Time.deltaTime;
            if (countTimerApplyDamage > cooldownApplyDamage)
            {
                ApplyDamage();
                isAttacked = true;
                countTimerApplyDamage = 0;
            }
            unitEndurance.EnduranceStat.RemoveValue(ReduceEnduranceStat.CurrentValue);
        }

        protected virtual void ApplyDamage()
        {
            CurrentWeapon?.ApplyDamage();
        }
    }

    public class CharacterWeaponAttackStateBuilder : StateBuilder<CharacterWeaponAttackState>
    {
        public CharacterWeaponAttackStateBuilder(CharacterWeaponAttackState instance) : base(instance)
        {
        }
        
        public CharacterWeaponAttackStateBuilder SetConfig(SO_CharacterAttack config)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetConfig(config);

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetGameObject(gameObject);

            return this;
        }
        public CharacterWeaponAttackStateBuilder SetCenter(Transform center)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetCenter(center);

            return this;
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
        public CharacterWeaponAttackStateBuilder SetUnitRenderer(UnitRenderer unitRenderer)
        {
            if (state is CharacterWeaponAttackState characterWeapon)
                characterWeapon.SetUnitRenderer(unitRenderer);

            return this;
        }
    }
}