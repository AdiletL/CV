﻿using System;
using Gameplay.Damage;
using ScriptableObjects.Unit.Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Unit.Character
{
    public class CharacterAttackState : UnitAttackState
    {
        protected SO_CharacterAttack so_CharacterAttack;
        protected GameObject currentTarget;
        protected Transform weaponParent;
        protected UnitAnimation unitAnimation;
        protected UnitEndurance unitEndurance;
        protected UnitRenderer unitRenderer;
        protected AnimationClip currentClip;
        protected LayerMask enemyLayer;
        
        protected Collider[] findUnitColliders = new Collider[1];
        
        protected float durationAttack, countDurationAttack;
        protected float cooldownApplyDamage, countTimerApplyDamage;
        protected float angleToTarget;
        
        protected bool isAttacked;
        
        protected const string ATTACK_SPEED_NAME = "SpeedAttack";
        protected const int ANIMATION_LAYER = 1;
        
        public Equipment.Weapon.Weapon CurrentWeapon { get; protected set; }
        public Stat ReduceEnduranceStat { get; } = new();
        public Stat RangeStat { get; } = new();

        ~CharacterAttackState()
        {
            UnsubscribeStatEvent();
        }

        public void SetConfig(SO_CharacterAttack config) => so_CharacterAttack = config;
        public void SetWeaponParent(Transform parent) => weaponParent = parent;
        public void SetUnitAnimation(UnitAnimation animation) => unitAnimation = animation;
        public void SetUnitEndurance(UnitEndurance endurance) => unitEndurance = endurance;
        public void SetUnitRenderer(UnitRenderer unitRenderer) => this.unitRenderer = unitRenderer;
        
        protected override IDamageable CreateDamageable()
        {
            return new NormalDamage(gameObject, so_CharacterAttack.Damage);
        }
        
        public virtual bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<IAttackable>(center.position, RangeStat.CurrentValue, enemyLayer, ref findUnitColliders);
        }
        
        protected virtual AnimationEventConfig getAnimationEventConfig()
        {
            return so_CharacterAttack.DefaultAnimations[Random.Range(0, so_CharacterAttack.DefaultAnimations.Length)];
        }
        
        public override void Initialize()
        {
            base.Initialize();
            SubscribeStatEvent();
            
            enemyLayer = so_CharacterAttack.EnemyLayer;
            angleToTarget = so_CharacterAttack.AngleToTarget;
            
            DamageStat.AddValue(so_CharacterAttack.Damage);
            ReduceEnduranceStat.AddValue(so_CharacterAttack.BaseReductionEndurance);
            AttackSpeedStat.AddValue(so_CharacterAttack.AttackSpeed);
            RangeStat.AddValue(so_CharacterAttack.Range);

            for (int i = 0; i < so_CharacterAttack.DefaultAnimations.Length; i++)
                unitAnimation.AddClip(so_CharacterAttack.DefaultAnimations[i].Clip);
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
            this.unitAnimation?.ExitAnimation(ANIMATION_LAYER);
            currentTarget = null;
        }

        protected virtual void SubscribeStatEvent()
        {
            AttackSpeedStat.OnAddCurrentValue += OnAddAttackSpeedStatCurrentValue;
            AttackSpeedStat.OnRemoveCurrentValue += OnRemoveAttackSpeedStatCurrentValue;
        }

        protected virtual void UnsubscribeStatEvent()
        {
            AttackSpeedStat.OnAddCurrentValue -= OnAddAttackSpeedStatCurrentValue;
            AttackSpeedStat.OnRemoveCurrentValue -= OnRemoveAttackSpeedStatCurrentValue;
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
        }

        protected void UpdateCurrentClip()
        {
            var config = getAnimationEventConfig();
            currentClip = config.Clip;
            cooldownApplyDamage = durationAttack * config.MomentEvent;
        }
        
        public virtual void SetWeapon(Equipment.Weapon.Weapon weapon)
        {
            if(weapon == CurrentWeapon) return;
            RemoveWeapon();
            
            CurrentWeapon = weapon;
            CurrentWeapon.SetInParent(weaponParent);
            CurrentWeapon.SetEnemyLayer(enemyLayer);
            CurrentWeapon.SetOwnerDamageStat(DamageStat);
            CurrentWeapon.SetOwnerRangeStat(RangeStat);
            CurrentWeapon.Show();
            ClearValues();
            UpdateDurationAttack();
            ReduceEnduranceStat.AddValue(CurrentWeapon.ReduceEndurance);
        }

        public void RemoveWeapon()
        {
            if(CurrentWeapon == null) return;
            ReduceEnduranceStat.RemoveValue(CurrentWeapon.ReduceEndurance);
            CurrentWeapon.SetOwnerDamageStat(null);
            CurrentWeapon.SetOwnerRangeStat(null);
            CurrentWeapon.Hide();
            CurrentWeapon = null;
        }

        protected void FindUnitInRange()
        {
            var target = Calculate.Attack.FindUnitInRange(center.position, RangeStat.CurrentValue,
                enemyLayer, ref findUnitColliders);
            if(target == null) return;
            
            var directionToTarget = (target.GetComponent<UnitCenter>().Center.position - center.position).normalized;
            
            //Debug.DrawRay(origin, directionToTarget * 100, Color.green, 2);
            if (Physics.Raycast(center.position, directionToTarget, out var hit, RangeStat.CurrentValue, ~gameObject.layer))
            {
                if(hit.collider.gameObject.layer == target.gameObject.layer)
                    currentTarget = target;
            }
        }
        
        public override void Attack()
        {
            if(isAttacked) return;
            
            this.unitAnimation?.ChangeAnimationWithDuration(currentClip, duration: durationAttack, ATTACK_SPEED_NAME, layer: ANIMATION_LAYER);
            
            countTimerApplyDamage += Time.deltaTime;
            if (countTimerApplyDamage > cooldownApplyDamage)
            {
                ApplyDamage();
                isAttacked = true;
                countTimerApplyDamage = 0;
            }
            unitEndurance.EnduranceStat.RemoveValue(ReduceEnduranceStat.CurrentValue);
        }
        
        public override void ApplyDamage()
        {
            if (CurrentWeapon != null)
                CurrentWeapon.ApplyDamage();
            else
                DefaultApplyDamage();
        }

        protected virtual void DefaultApplyDamage()
        {
            
        }
    }

    public class CharacterAttackStateBuilder : UnitAttackStateBuilder
    {
        public CharacterAttackStateBuilder(CharacterAttackState instance) : base(instance)
        {
        }
        
        public CharacterAttackStateBuilder SetConfig(SO_CharacterAttack config)
        {
            if (state is CharacterAttackState characterWeapon)
                characterWeapon.SetConfig(config);
            return this;
        }
        public CharacterAttackStateBuilder SetUnitAnimation(UnitAnimation unitAnimation)
        {
            if (state is CharacterAttackState characterWeapon)
                characterWeapon.SetUnitAnimation(unitAnimation);
            return this;
        }

        public CharacterAttackStateBuilder SetWeaponParent(Transform parent)
        {
            if (state is CharacterAttackState characterWeapon)
                characterWeapon.SetWeaponParent(parent);
            return this;
        }
        public CharacterAttackStateBuilder SetUnitEndurance(UnitEndurance endurance)
        {
            if (state is CharacterAttackState characterWeapon)
                characterWeapon.SetUnitEndurance(endurance);
            return this;
        }
        public CharacterAttackStateBuilder SetUnitRenderer(UnitRenderer unitRenderer)
        {
            if (state is CharacterAttackState characterWeapon)
                characterWeapon.SetUnitRenderer(unitRenderer);
            return this;
        }
    }
}