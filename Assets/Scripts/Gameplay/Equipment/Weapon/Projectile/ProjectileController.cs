﻿using System;
using Gameplay.Manager;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Weapon.Projectile
{
    public abstract class ProjectileController : MonoBehaviour, IProjectile
    {
        [Inject] protected PoolManager poolManager;

        [SerializeField] protected SO_Projectile so_Projectile;
        
        protected GameObject target;
        protected LayerMask enemyLayer;
        

        public IDamageable Damageable { get; protected set; }
        public AnimationCurve MoveCurve { get; protected set; }
        public Stat MovementSpeedStat { get; protected set; } = new Stat();

        protected float height;
        protected bool isInitialized;
        
        public void SetDamageable(IDamageable damageable) => Damageable = damageable;

        public virtual void Initialize()
        {
            if(isInitialized) return;
            
            MoveCurve = so_Projectile.Curve;
            MovementSpeedStat.AddValue(so_Projectile.Speed);
            height = so_Projectile.Height;
            enemyLayer = so_Projectile.EnemyLayer;
        }

        private void Update()
        {
            if(!isInitialized) return;
            ExecuteMovement();
        }
        public abstract void ExecuteMovement();


        public virtual void ApplyDamage()
        {
            if (target &&
                target.TryGetComponent(out IAttackable attackable) &&
                target.TryGetComponent(out IHealth health))
            {
                if (health.IsLive)
                {
                    attackable.TakeDamage(Damageable);
                }
            }

            ReturnToPool();
        }

        public void SetTriggerTarget(GameObject target)
        {
            this.target = target;
        }
        
        protected void ReturnToPool()
        {
            poolManager.ReturnToPool(gameObject);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(enemyLayer, other.gameObject.layer)
            || other.gameObject == Damageable.Owner || 
            other.TryGetComponent(out RoomController roomController)) return;
            
            SetTriggerTarget(other.gameObject);
            ApplyDamage();
        }
    }
}