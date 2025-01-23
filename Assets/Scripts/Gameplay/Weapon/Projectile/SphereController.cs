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
        private IMovement forwardMovement;
        private IEffect slowMovementEffect;

        public override void Initialize()
        {
            base.Initialize();
            so_Sphere = (SO_Sphere)so_Projectile;
            
            if (forwardMovement == null)
                forwardMovement = new ForwardMovement(gameObject, MovementSpeed);

            if (slowMovementEffect == null)
                slowMovementEffect = new SlowMovement(so_Sphere.SlowMovementInfo);
            
            forwardMovement.Initialize();
        }

        public override void ExecuteMovement()
        {
            if(forwardMovement == null) return;
            forwardMovement.ExecuteMovement();
        }

        public override void ApplyDamage()
        {
            base.ApplyDamage();
            var handleEffect = target.GetComponent<EffectHandler>();
            if(handleEffect == null || handleEffect.IsEffectActive(slowMovementEffect)) return;
            
            slowMovementEffect.SetTarget(target);
            slowMovementEffect.ApplyEffect();
            handleEffect.AddEffect(slowMovementEffect);
        }
    }
}