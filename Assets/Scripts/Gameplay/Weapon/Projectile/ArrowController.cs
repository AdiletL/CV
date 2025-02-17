using System;
using Movement;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    public class ArrowController : ProjectileController
    {
        private DirectionMovement directionMovement;
        private Vector3 startPosition;
        private float rangeSqr;
        private float distance;
        
        public override void Initialize()
        {
            base.Initialize();
            if(isInitialized) return;
            
            directionMovement = new DirectionMovement(gameObject, CurrentMovementSpeed);
            directionMovement.Initialize();
            
            isInitialized = true;
        }
        private void OnFinished()
        {
            ReturnToPool();
        }

        public override void ExecuteMovement()
        {
            distance = (startPosition - transform.position).sqrMagnitude;
            if (distance < rangeSqr)
                directionMovement.ExecuteMovement();
            else
                ReturnToPool();
        }

        public void UpdateData(Vector3 direction, float range)
        {
            directionMovement.SetDirection(direction);
            startPosition = transform.position;
            this.rangeSqr = range * range;
        }
    }
}