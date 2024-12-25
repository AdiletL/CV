using Gameplay.Effect;
using Movement;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    [RequireComponent(typeof(SphereCollider))]
    public class SphereController : ProjectileController
    {
        private SO_Sphere so_Sphere;
        private ForwardMovement forwardMovement;
        private Gameplay.Effect.Effect slowMovementEffect;

        public override void Initialize()
        {
            base.Initialize();
            so_Sphere = (SO_Sphere)so_Projectile;
            
            if (forwardMovement == null)
                forwardMovement = new ForwardMovement(gameObject, MovementSpeed);

            if (slowMovementEffect == null)
                slowMovementEffect = new SlowMovement(so_Sphere.decreaseSpeed, so_Sphere.durationEffect);
            
            forwardMovement.Initialize();
        }

        public override void Move()
        {
            forwardMovement.Move();
        }

        public override void ApplyDamage()
        {
            base.ApplyDamage();
            var handleEffect = target.GetComponent<HandleEffect>();
            if(handleEffect == null) return;
            
            slowMovementEffect.SetTarget(target);
            slowMovementEffect.ApplyEffect();
            handleEffect.AddEffect(slowMovementEffect);
        }
    }
}