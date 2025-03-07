﻿using Gameplay.Effect;
using Gameplay.Movement;
using Movement;
using ScriptableObjects.Weapon.Projectile;
using UnityEngine;

namespace Gameplay.Weapon.Projectile
{
    [RequireComponent(typeof(SphereCollider))]
    public class SphereController : ProjectileController
    {
        private SO_Sphere so_Sphere;
        private IMovement directionMovement;
        private IEffect slowMovementEffect;

        public override void Initialize()
        {
            base.Initialize();
            if(isInitialized) return;
            
            so_Sphere = (SO_Sphere)so_Projectile;
            
            if (directionMovement == null)
                directionMovement = new DirectionMovement(gameObject, MovementSpeedStat.CurrentValue);

            if (slowMovementEffect == null)
                slowMovementEffect = new SlowMovement(so_Sphere.SlowMovementInfo);
            
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
            if(handleEffect == null || handleEffect.IsEffectActive(slowMovementEffect)) return;
            
            slowMovementEffect.SetTarget(target);
            slowMovementEffect.ApplyEffect();
            handleEffect.AddEffect(slowMovementEffect);
        }
    }
}