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


        public  void Trigger()
        {
            axeAnimation.ChangeAnimationWithDuration(base.playClip);
        }

        public  void Reset()
        {
            axeAnimation.ChangeAnimationWithDuration(resetClip);
        }

        public void ChangeOnPlayClip()
        {
            axeAnimation.ChangeAnimationWithSpeed(playClip, speedPlayClip);
        }

        private void OnEnable()
        {
            //GetComponentInUnit<AxeCollision>().OnHitEnter += OnHitEnter;
        }
        private void OnDisable()
        {
            //GetComponentInUnit<AxeCollision>().OnHitEnter -= OnHitEnter;
        }

        private void OnHitEnter(GameObject target)
        {
            //SetTarget(target);
            ApplyDamage();
        }


        public void ApplyDamage()
        {
            
        }
    }
}
