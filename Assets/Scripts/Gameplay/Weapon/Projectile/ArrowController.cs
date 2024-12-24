using System;
using Movement;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    public class ArrowController : ProjectileController
    {
        private ArcMovement arcMovement;
        
        public override void Initialize()
        {
            base.Initialize();
            
            if(arcMovement == null)
                arcMovement = new ArcMovement(gameObject, MovementSpeed, height, moveCurve);
            
            arcMovement.Initialize();

            arcMovement.OnFinished += OnFinished;
        }
        private void OnFinished()
        {
            ReturnToPool();
        }

        private void OnDisable()
        {
            arcMovement.OnFinished -= OnFinished;
        }


        public override void Move()
        {
            arcMovement.Move();
        }

        public void UpdateData(Vector3 targetPosition)
        {
            arcMovement.UpdateData(targetPosition);
        }
    }
}