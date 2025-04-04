using System;
using System.Collections.Generic;
using Gameplay.Ability;
using Gameplay.Spawner;
using Gameplay.Unit;
using Gameplay.Unit.Item;
using ScriptableObjects.Gameplay.Equipment.Weapon;
using UnityEngine;
using Zenject;

namespace Gameplay.Equipment.Weapon
{
    public abstract class Weapon : Equipment, IWeapon
    {
        [Inject] private DiContainer diContainer;
        [Inject] private CriticalDamagePopUpSpawner criticalDamagePopUpSpawner;
        
        public override EquipmentType EquipmentTypeID { get; protected set; } = EquipmentType.Weapon;

        protected SO_Weapon so_Weapon;
        protected GameObject currentTarget;
        protected LayerMask enemyLayer;
        protected float angleToTarget;

        public DamageData DamageData { get; protected set; }
        public Stat DamageStat { get; } = new();
        public Stat RangeStat { get; } = new();
        public Stat OwnerDamageStat { get; protected set; } = new();
        public Stat OwnerRangeStat { get; protected set; } = new();
        
        public int SpecialActionIndex { get; protected set; }
        public bool IsActivatedSpecialAction { get; protected set; }

        public Weapon(SO_Weapon so_Weapon) : base(so_Weapon)
        {
            
        }
        
        public void SetEnemyLayer(LayerMask layer) => enemyLayer = layer;
        public void SetOwnerDamageStat(Stat damageStat) => this.OwnerDamageStat = damageStat;
        public void SetOwnerRangeStat(Stat rangeStat) => this.OwnerRangeStat = rangeStat;

        
        public override void Initialize()
        {
            base.Initialize();
            so_Weapon = (SO_Weapon)so_Equipment;
            angleToTarget = so_Weapon.AngleToTarget;
            
            DamageData = new DamageData(Owner, DamageType.Physical, DamageStat.CurrentValue);
            diContainer.Inject(DamageData);
        }

        public void SetTarget(GameObject target) => currentTarget = target;

        public abstract void ApplyDamage();

        protected void CheckCriticalDamage(DamageData damageData, ref bool isWasCriticalApplied, IAttackable attackable, GameObject target)
        {
            if (damageData.DamageTypeID.HasFlag(DamageType.Physical))
            {
                var criticalDamageProviders = damageData.Owner.GetComponents<ICriticalDamageProvider>();
                for (int i = 0; i < criticalDamageProviders.Length; i++)
                {
                    var criticalDamages = criticalDamageProviders[i].GetCriticalDamages(damageData.Amount);
                    if (criticalDamages != null && criticalDamages.Count > 0)
                    {
                        ExecuteCriticalDamage(damageData, criticalDamages, attackable, target);
                        isWasCriticalApplied = true;
                    }
                }
            }
        }

        protected void ExecuteCriticalDamage(DamageData damageData, List<float> criticalDamages, IAttackable attackable, GameObject target)
        {
            var unitCenter = target.GetComponent<UnitCenter>();
            foreach (var crit in criticalDamages)
            {
                var critData = damageData.Clone();
                critData.Amount = crit;
                criticalDamagePopUpSpawner.CreatePopUp(unitCenter.Center.position, critData.Amount);
                attackable.TakeDamage(critData);
            }
        }
    }
}