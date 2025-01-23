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
            if(isInitialized) return;
            
            arcMovement = new ArcMovement(gameObject, MovementSpeed, height, moveCurve);
            arcMovement.Initialize();
            
            isInitialized = true;
            arcMovement.OnFinished += OnFinished;
        }
        private void OnFinished()
        {
            ReturnToPool();
        }

        public override void ExecuteMovement()
        {
            arcMovement.ExecuteMovement();
        }

        public void UpdateData(Vector3 targetPosition)
        {
            arcMovement.UpdateData(targetPosition);
        }
        
        private void OnDestroy()
        {
            if(arcMovement != null)
                arcMovement.OnFinished -= OnFinished;
        }
    }
}