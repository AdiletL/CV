using Cysharp.Threading.Tasks;
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
            DamageData.Amount = DamageStat.CurrentValue + OwnerDamageStat.CurrentValue;
            arrow.SetDamageable(DamageData);
            arrow.Initialize();
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