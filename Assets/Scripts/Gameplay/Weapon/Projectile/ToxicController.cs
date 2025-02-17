using Movement;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    [RequireComponent(typeof(SphereCollider))]
    public class ToxicController : ProjectileController
    {
        private DirectionMovement directionMovement;
        
        public override void Initialize()
        {
            base.Initialize();
            if (directionMovement == null)
                directionMovement = new DirectionMovement(gameObject, CurrentMovementSpeed);
            
            directionMovement.Initialize();
            directionMovement.SetDirection(Vector3.forward);
        }
        
        public override void ExecuteMovement()
        {
            directionMovement.ExecuteMovement();
        }

        public override void ApplyDamage()
        {
            
        }
    }
}