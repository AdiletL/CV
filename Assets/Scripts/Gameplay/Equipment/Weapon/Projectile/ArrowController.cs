using System;
using Movement;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    public class ArrowController : ProjectileController
    {
        private DirectionMovement directionMovement;
        private float countTimerReturnToPool;
        private const float TIMER_RETURN_TO_POOL = 5f;
        
        public override void Initialize()
        {
            base.Initialize();
            if(isInitialized) return;
            
            directionMovement = new DirectionMovement(gameObject, MovementSpeedStat.CurrentValue);
            directionMovement.Initialize();
            directionMovement.SetDirection(Vector3.forward);
            
            isInitialized = true;
        }

        public void ClearValues()
        {
            countTimerReturnToPool = 0;
        }
        public override void ExecuteMovement()
        {
            countTimerReturnToPool += Time.deltaTime;
            if(countTimerReturnToPool < TIMER_RETURN_TO_POOL)
                directionMovement.ExecuteMovement();
            else
                ReturnToPool();
        }
    }
}