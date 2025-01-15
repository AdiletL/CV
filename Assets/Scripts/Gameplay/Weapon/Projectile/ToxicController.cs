using Movement;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    [RequireComponent(typeof(SphereCollider))]
    public class ToxicController : ProjectileController
    {
        private ForwardMovement forwardMovement;
        
        public override void Initialize()
        {
            base.Initialize();
            if (forwardMovement == null)
                forwardMovement = new ForwardMovement(gameObject, MovementSpeed);
            
            forwardMovement.Initialize();
        }
        
        public override void ExecuteMovement()
        {
            forwardMovement.ExecuteMovement();
        }

        public override void ApplyDamage()
        {
            
        }
    }
}