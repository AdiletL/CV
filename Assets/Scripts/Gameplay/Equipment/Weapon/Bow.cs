using Cysharp.Threading.Tasks;
using Gameplay.Damage;
using Gameplay.Manager;
using Gameplay.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Equipment.Weapon
{
    public class Bow : Weapon
    {
        [Inject] private PoolManager pool;
        
        private Transform pointSpawnProjectile;
        
        protected override IDamageable CreateDamageable()
        {
            return new NormalDamage(owner, DamageStat);
        }

        public override void Initialize()
        {
            base.Initialize();
            pointSpawnProjectile = equipment.transform.GetChild(0);
        }

        public override async void ApplyDamage()
        {
            var newGameObject = await this.pool.GetObjectAsync<ArrowController>();
            newGameObject.transform.position = pointSpawnProjectile.position;
            newGameObject.transform.rotation = pointSpawnProjectile.rotation;
            var arrow = newGameObject.GetComponent<ArrowController>();
            arrow.Initialize();
            arrow.SetDamageable(Damageable);
            arrow.ClearValues();
        }
    }
    
    public class BowBuilder : WeaponBuilder
    {
        public BowBuilder() : base(new Bow())
        {
        }
    }
}