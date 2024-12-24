using System;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Weapon.Projectile
{
    public abstract class ProjectileController : MonoBehaviour, IProjectile
    {
        [SerializeField] protected SO_Projectile so_Projectile;

        protected IPoolable poolable;
        protected GameObject target;
        
        public IDamageable Damageable { get; private set; }
        public AnimationCurve moveCurve { get; protected set; }
        public float MovementSpeed { get; protected set; }

        protected float height;
        
        [Inject]
        public void Construct(IPoolable poolable)
        {
            this.poolable = poolable;
        }


        public virtual void Initialize()
        {
            moveCurve = so_Projectile.Curve;
            MovementSpeed = so_Projectile.Speed;
            height = so_Projectile.Height;
        }

        private void Update()
        {
            Move();
        }
        public abstract void Move();

        public virtual void ApplyDamage()
        {
            if (target && target.TryGetComponent(out IHealth health))
            {
                if(health.IsLive)
                    health.TakeDamage(Damageable);
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
            poolable.ReturnToPool(gameObject);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out ProjectileController projectileController) 
            || other.gameObject == Damageable.Owner) return;
            
            SetTriggerTarget(other.gameObject);
            ApplyDamage();
        }
    }
}