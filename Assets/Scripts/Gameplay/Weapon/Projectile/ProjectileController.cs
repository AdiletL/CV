using System;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Weapon.Projectile
{
    public abstract class ProjectileController : MonoBehaviour, IProjectile
    {
        [Inject] protected IPoolableObject IPoolableObject;

        [SerializeField] protected SO_Projectile so_Projectile;
        
        protected GameObject target;
        protected LayerMask enemyLayer;
        
        public IDamageable Damageable { get; private set; }
        public AnimationCurve moveCurve { get; protected set; }
        public float MovementSpeed { get; protected set; }

        protected float height;
        

        public virtual void Initialize()
        {
            moveCurve = so_Projectile.Curve;
            MovementSpeed = so_Projectile.Speed;
            height = so_Projectile.Height;
            enemyLayer = so_Projectile.EnemyLayer;
        }

        private void Update()
        {
            ExecuteMovement();
        }
        public abstract void ExecuteMovement();

        public virtual void ApplyDamage()
        {
            if (target &&
                target.TryGetComponent(out IAttackable attackable) &&
                target.TryGetComponent(out IHealth health))
            {
                if(health.IsLive)
                    attackable.TakeDamage(Damageable);
            }

            ReturnToPool();
        }

        public void SetTriggerTarget(GameObject target)
        {
            this.target = target;
        }

        public void SetDamageable(IDamageable damageable)
        {
            Damageable = damageable;
        }

        protected void ReturnToPool()
        {
            IPoolableObject.ReturnToPool(gameObject);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(enemyLayer, other.gameObject.layer)
            || other.gameObject == Damageable.Owner) return;
            
            SetTriggerTarget(other.gameObject);
            ApplyDamage();
        }
    }
}