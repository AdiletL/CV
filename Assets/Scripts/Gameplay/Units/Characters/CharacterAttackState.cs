using System;
using System.Collections.Generic;
using ScriptableObjects.Unit.Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Unit.Character
{
    public class CharacterAttackState : UnitAttackState
    {
        protected SO_CharacterAttack so_CharacterAttack;
        protected GameObject currentTarget;
        protected UnitAnimation unitAnimation;
        protected UnitRenderer unitRenderer;
        protected AnimationClip currentClip;
        protected LayerMask enemyLayer;

        protected RaycastHit hitInTarget;
        protected Collider[] findUnitColliders = new Collider[1];
        
        protected int mainLayer;
        protected int currentAnimatonLayer;
        protected int specialActionConfigIndex;
        
        protected float durationAttack, countDurationAttack;
        protected float angleToTarget;
        
        protected bool isAttacked;
       
        protected Queue<float> cooldownsApplyDamage;
        
        protected const string ATTACK_SPEED_NAME = "SpeedAttack";
        protected const int DEFAULT_ANIMATION_LAYER = 1;
        protected const int SPECIAL_ANIMATION_LAYER = 2;
        
        public Equipment.Weapon.Weapon CurrentWeapon { get; protected set; }
        public Stat ConsumptionEnduranceStat { get; } = new();

        ~CharacterAttackState()
        {
            UnsubscribeStatEvent();
        }

        public void SetConfig(SO_CharacterAttack config) => so_CharacterAttack = config;
        public void SetUnitAnimation(UnitAnimation animation) => unitAnimation = animation;
        public void SetUnitRenderer(UnitRenderer unitRenderer) => this.unitRenderer = unitRenderer;
        
        public virtual bool IsUnitInRange()
        {
            currentTarget = FindUnitInRange<IAttackable>();
            return currentTarget;
        }
        
        protected GameObject FindUnitInRange<T>()
        {
            var totalRange = RangeAttackStat.CurrentValue;
            if (CurrentWeapon != null)
                totalRange += CurrentWeapon.RangeStat.CurrentValue;

            var target = Calculate.Attack.FindUnitInRange<T>(center.position, totalRange,
                enemyLayer, ref findUnitColliders);
            if(!target) return null;

            if (!isObstacleBetween(target))
                return target;
                
            return null;
        }

        protected bool isObstacleBetween(GameObject target)
        {
            var directionToTarget = (target.GetComponent<UnitCenter>().Center.position - center.position).normalized;
            float distance = Vector3.Distance(center.position, target.transform.position);
            int ignoreLayer = 1 << mainLayer;
 
            if (Physics.Raycast(center.position,  directionToTarget, out hitInTarget, distance, ~ignoreLayer))
            {
                if(hitInTarget.collider.gameObject.layer == target.gameObject.layer)
                    return false;
            }
            return true;
        }
        
        protected virtual AnimationEventConfig getAnimationEventConfig()
        {
            return so_CharacterAttack.DefaultAnimations[Random.Range(0, so_CharacterAttack.DefaultAnimations.Length)];
        }

        protected virtual AnimationEventConfig getSpecialAnimationEventConfig()
        {
            return null;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            SubscribeStatEvent();
            
            enemyLayer = so_CharacterAttack.EnemyLayer;
            angleToTarget = so_CharacterAttack.AngleToTarget;
            
            DamageStat.AddCurrentValue(so_CharacterAttack.Damage);
            ConsumptionEnduranceStat.AddCurrentValue(so_CharacterAttack.ConsumptionEnduranceRate);
            AttackSpeedStat.AddCurrentValue(so_CharacterAttack.AttackSpeed);
            RangeAttackStat.AddCurrentValue(so_CharacterAttack.Range);

            for (int i = 0; i < so_CharacterAttack.DefaultAnimations.Length; i++)
                unitAnimation.AddClip(so_CharacterAttack.DefaultAnimations[i].Clip);

            mainLayer = gameObject.layer;
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
        
        public override void Exit()
        {
            base.Exit();
            this.unitAnimation.ExitAnimation(DEFAULT_ANIMATION_LAYER);
            currentTarget = null;
        }

        protected virtual void SubscribeStatEvent()
        {
            AttackSpeedStat.OnChangedCurrentValue += OnChangedAttackSpeedStatCurrentValue;
        }

        protected virtual void UnsubscribeStatEvent()
        {
            AttackSpeedStat.OnChangedCurrentValue -= OnChangedAttackSpeedStatCurrentValue;
        }
        private void OnChangedAttackSpeedStatCurrentValue() => UpdateDurationAttack();
        

        public void SetTarget(GameObject target) => currentTarget = target;
        
        protected virtual void ClearValues()
        {
            countDurationAttack = 0;
            cooldownsApplyDamage?.Clear();
            isAttacked = false;
        }

        protected void UpdateDurationAttack()
        {
            durationAttack = Calculate.Convert.AttackSpeedToDuration(AttackSpeedStat.CurrentValue);
        }

        protected virtual void UpdateCurrentClip()
        {
            AnimationEventConfig config = null;
            if (CurrentWeapon != null && CurrentWeapon.IsActivatedSpecialAction)
            {
                specialActionConfigIndex = CurrentWeapon.SpecialActionIndex;
                config = getSpecialAnimationEventConfig();
                currentClip = config.Clip;
                currentAnimatonLayer = SPECIAL_ANIMATION_LAYER;
            }
            else
            {
                config = getAnimationEventConfig();
                currentClip = config.Clip;
                currentAnimatonLayer = DEFAULT_ANIMATION_LAYER;
            }
            
            cooldownsApplyDamage ??= new Queue<float>();
            cooldownsApplyDamage.Clear();
            for (int i = 0; i < config.MomentEvents.Length; i++)
                cooldownsApplyDamage.Enqueue(durationAttack * config.MomentEvents[i]);
        }
        
        public virtual void SetWeapon(Equipment.Weapon.Weapon weapon)
        {
            if(weapon == CurrentWeapon) return;
            RemoveWeapon();
            
            CurrentWeapon = weapon;
            CurrentWeapon.SetEnemyLayer(enemyLayer);
            CurrentWeapon.SetOwnerDamageStat(DamageStat);
            CurrentWeapon.SetOwnerRangeStat(RangeAttackStat);
            CurrentWeapon.Show();
            ClearValues();
            UpdateDurationAttack();
        }

        public void RemoveWeapon()
        {
            if(CurrentWeapon == null) return;
            CurrentWeapon.SetOwnerDamageStat(null);
            CurrentWeapon.SetOwnerRangeStat(null);
            CurrentWeapon.Hide();
            CurrentWeapon = null;
        }
        
        public override void Attack()
        {
            if(isAttacked) return;

            this.unitAnimation.ChangeAnimationWithDuration(currentClip, duration: durationAttack, ATTACK_SPEED_NAME, layer: currentAnimatonLayer);
            
            countDurationAttack += Time.deltaTime;
            if (durationAttack < countDurationAttack)
            {
                FinishedAttack();
                countDurationAttack = 0;
            }
            else
            {
                if (cooldownsApplyDamage.Count > 0)
                {
                    if (cooldownsApplyDamage.Peek() <= countDurationAttack)
                    {
                        ApplyDamage();
                        cooldownsApplyDamage.Dequeue();
                    }
                }
            }
        }

        protected virtual void FinishedAttack()
        {
            stateMachine.ExitCategory(Category, null);
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
            if(currentTarget &&
               !isObstacleBetween(currentTarget) &&
               Calculate.Rotate.IsFacingTargetXZ(gameObject.transform.position,
                   gameObject.transform.forward, currentTarget.transform.position, angleToTarget) &&
               currentTarget.TryGetComponent(out IAttackable attackable) && 
               currentTarget.TryGetComponent(out IHealth health) && health.IsLive)
            {
                DamageData.Amount = DamageStat.CurrentValue;
                attackable.TakeDamage(DamageData);
            }
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

        public CharacterAttackStateBuilder SetUnitRenderer(UnitRenderer unitRenderer)
        {
            if (state is CharacterAttackState characterWeapon)
                characterWeapon.SetUnitRenderer(unitRenderer);
            return this;
        }
    }
}