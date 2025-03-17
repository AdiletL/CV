using System;
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
        
        public DamageData DamageData { get; private set; }
        public Stat DamageStat { get; } = new Stat();
        

        public override void Initialize()
        {
            base.Initialize();
            DamageStat.AddCurrentValue(so_Trap.Damage);
            DamageData = new DamageData(gameObject, DamageType.Physical, DamageStat.CurrentValue);
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


        public override void Trigger()
        {
            axeAnimation.ChangeAnimationWithDuration(appearClip);
        }

        public override void Reset()
        {
            axeAnimation.ChangeAnimationWithDuration(deappearClip);
        }

        public void ChangeOnPlayClip()
        {
            axeAnimation.ChangeAnimationWithSpeed(playClip, speedPlayClip);
        }

        private void OnEnable()
        {
            GetComponentInUnit<AxeTrigger>().OnHitEnter += OnHitEnter;
        }
        private void OnDisable()
        {
            GetComponentInUnit<AxeTrigger>().OnHitEnter -= OnHitEnter;
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
                trapAttackable.TakeDamage(DamageData);
                CurrentTarget = null;
            }
        }
    }
}
