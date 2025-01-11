using Cysharp.Threading.Tasks;
using Gameplay.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Weapon
{
    public class Bow : Weapon
    {
        private IPoolable pool;
        private Transform pointSpawnProjectile;

        [Inject]
        public void Construct(IPoolable pool)
        {
            this.pool = pool;
        }

        public override void Initialize()
        {
            base.Initialize();
            pointSpawnProjectile = weapon.transform.GetChild(0);
        }

        public override async UniTask FireAsync()
        {
            var newGameObject = await this.pool.GetObjectAsync<ArrowController>();
            newGameObject.transform.position = pointSpawnProjectile.position;
            newGameObject.transform.rotation = pointSpawnProjectile.rotation;
            var arrow = newGameObject.GetComponent<ArrowController>();
            arrow.Initialize();
            arrow.SetDamageable(Damageable);
            arrow.UpdateData(CurrentTarget.transform.position);
        }
    }
    
    public class BowBuilder : WeaponBuilder
    {
        public BowBuilder() : base(new Bow())
        {
        }
    }
}