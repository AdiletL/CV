using Gameplay.Effect;
using Gameplay.Movement;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    [RequireComponent(typeof(SphereCollider))]
    public class SphereController : ProjectileController
    {
        private SO_Sphere so_Sphere;
        private IMovement directionMovement;
        private SlowMovementEffect slowMovementEffect;

        public override void Initialize()
        {
            base.Initialize();
            if(isInitialized) return;
            
            so_Sphere = (SO_Sphere)so_Projectile;

            if (directionMovement == null)
            {
                directionMovement = new DirectionMovement(gameObject, MovementSpeedStat.CurrentValue);
                directionMovement.Initialize();
            }

            if (slowMovementEffect == null)
                slowMovementEffect = new SlowMovementEffect(so_Sphere.EffectConfigData.SlowMovementConfig);
            
            isInitialized = true;
        }

        public override void ExecuteMovement()
        {
            directionMovement.ExecuteMovement();
        }

        public override void ApplyDamage()
        {
            base.ApplyDamage();
            var handleEffect = target.GetComponent<EffectHandler>();
            if(handleEffect == null) return;
            
            var effect = handleEffect.GetEffect(slowMovementEffect);
            if (effect != null)
            {
                effect.UpdateEffect();
            }
            else
            {
                slowMovementEffect.SetTarget(target);
                handleEffect.AddEffect(slowMovementEffect);
            }
        }
    }
}