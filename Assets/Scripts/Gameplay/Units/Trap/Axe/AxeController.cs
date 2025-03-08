using System;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class AxeController : TrapController, IApplyDamage
    {
        private SO_Axe so_Axe;
        private AxeAnimation axeAnimation;
        private AnimationClip playClip;
        private float speedPlayClip;
        
        public IDamageable Damageable { get; private set; }
        public Stat DamageStat { get; private set; } = new Stat();
        

        public override void Initialize()
        {
            base.Initialize();
            DamageStat.AddValue(so_Trap.Damage);
            Damageable = new NormalDamage(gameObject, DamageStat.CurrentValue);
            axeAnimation = components.GetComponentFromArray<AxeAnimation>();
            so_Axe = (SO_Axe)so_Trap;
            speedPlayClip = so_Axe.SpeedPlayClip;
            playClip = so_Axe.PlayClip;
        }

        public override void Appear()
        {
            //Activate();
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
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
            if (CurrentTarget.TryGetComponent(out ITrapAttackable trapAttackable) &&
                CurrentTarget.TryGetComponent(out IHealth health) &&
                health.IsLive)
            {
                trapAttackable.TakeDamage(Damageable);
                CurrentTarget = null;
            }
        }
    }
}
