using System;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;
using Zenject;

namespace Gameplay.Weapon.Projectile
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField] protected SO_Projectile so_Projectile;

        protected IPoolable poolable;
        
        public AnimationCurve moveCurve { get; protected set; }
        public float MovementSpeed { get; protected set; }
        
        
        [Inject]
        public void Construct(IPoolable poolable)
        {
            this.poolable = poolable;
            moveCurve = so_Projectile.curve;
            MovementSpeed = so_Projectile.speed;
        }
        
        private void Update()
        {
            Move();
        }

        public abstract void Move();
    }
}