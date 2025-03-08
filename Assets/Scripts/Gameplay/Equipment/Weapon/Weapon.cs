using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Equipment.Weapon
{
    public abstract class Weapon : Equipment, IWeapon
    {
        [Inject] private DiContainer diContainer;
        
        protected GameObject currentTarget;
        protected LayerMask enemyLayer;
        protected float angleToTarget;

        public IDamageable Damageable { get; protected set; }
        public Stat DamageStat { get; } = new();
        public Stat RangeStat { get; } = new();
        public Stat OwnerDamageStat { get; protected set; } = new();
        public Stat OwnerRangeStat { get; protected set; } = new();
        public float ReduceEndurance { get; private set; }
        
        public void SetEnemyLayer(LayerMask layer) => enemyLayer = layer;
        public void SetAngleToTarget(float angle) => this.angleToTarget = angle;
        public void SetReduceEndurance(float reduceEndurance) => this.ReduceEndurance = reduceEndurance;
        public void SetOwnerDamageStat(Stat damageStat) => this.OwnerDamageStat = damageStat;
        public void SetOwnerRangeStat(Stat rangeStat) => this.OwnerRangeStat = rangeStat;

        protected abstract IDamageable CreateDamageable();
        
        public override void Initialize()
        {
            base.Initialize();
            Damageable = CreateDamageable();
            diContainer.Inject(Damageable);
        }
        

        public void SetTarget(GameObject target) => currentTarget = target;
            
        public abstract void ApplyDamage();
    }

    public abstract class WeaponBuilder : EquipmentBuilder<Weapon>
    {
        protected WeaponBuilder(Equipment equipment) : base(equipment)
        {
        }

        public WeaponBuilder SetAngleToTarget(float angle)
        {
            if(equipment is Weapon weapon)
                weapon.SetAngleToTarget(angle);
            return this;
        }
        public WeaponBuilder SetReduceEndurance(float value)
        {
            if(equipment is Weapon weapon)
                weapon.SetReduceEndurance(value);
            return this;
        }
    }
}