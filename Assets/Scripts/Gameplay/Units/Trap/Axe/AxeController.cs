using System;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Unit.Trap
{
    public class AxeController : TrapController, IApplyDamage
    {
        private SO_Axe so_Axe;
        private AxeAnimation axeAnimation;
        private AnimationClip playClip;
        private float speedPlayClip;
        
        public IDamageable Damageable { get; private set; }
        

        public override void Initialize()
        {
            base.Initialize();
            Damageable = new NormalDamage(so_Trap.Damage, gameObject);
            axeAnimation = components.GetComponentFromArray<AxeAnimation>();
            so_Axe = (SO_Axe)so_Trap;
            speedPlayClip = so_Axe.SpeedPlayClip;
            playClip = so_Axe.PlayClip;
            Activate();
        }

        public override void Appear()
        {
            
        }
        
        
        public override void Activate()
        {
            axeAnimation.ChangeAnimationWithDuration(activateClip);
        }

        public override void Deactivate()
        {
            axeAnimation.ChangeAnimationWithDuration(deactivateClip);
        }

        public void ChangeOnPlayClip()
        {
            axeAnimation.ChangeAnimationWithSpeed(playClip, speedPlayClip);
        }

        private void OnEnable()
        {
            GetComponentInUnit<AxeCollision>().OnHitEnter += OnHitEnter;
        }
        private void OnDisable()
        {
            GetComponentInUnit<AxeCollision>().OnHitEnter -= OnHitEnter;
        }

        private void OnHitEnter(GameObject target)
        {
            SetTarget(target);
            ApplyDamage();
        }


        public void ApplyDamage()
        {
            if (CurrentTarget.TryGetComponent(out IHealth health) 
                && health.IsLive)
            {
                health.TakeDamage(Damageable);
                CurrentTarget = null;
            }
        }
    }
}
