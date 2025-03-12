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
        private const string ID = nameof(SphereController);

        public override void Initialize()
        {
            base.Initialize();
            if(isInitialized) return;
            
            so_Sphere = (SO_Sphere)so_Projectile;
            
            if (directionMovement == null)
                directionMovement = new DirectionMovement(gameObject, MovementSpeedStat.CurrentValue);

            directionMovement.Initialize();
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
            
            var effect = handleEffect.GetEffect(EffectType.SlowMovement, ID);
            if (effect != null)
            {
                effect.UpdateEffect();
            }
            else
            {
                effect = new SlowMovementEffect(so_Sphere.EffectConfigData.SlowMovementConfig, ID);
                effect.SetTarget(target);
                handleEffect.AddEffect(effect);
            }
        }
    }
}