using System;
using ScriptableObjects.Gameplay.Equipment.Weapon;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Equipment.Weapon
{
    public abstract class Weapon : Equipment, IWeapon
    {
        [Inject] private DiContainer diContainer;

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
    }
}