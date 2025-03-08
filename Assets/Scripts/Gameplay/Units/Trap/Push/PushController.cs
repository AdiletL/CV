using System;
using System.Collections;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class PushController : TrapController, IApplyDamage
    {
        private SO_Push so_Push;
        private PushAnimation pushAnimation;
        private Coroutine startTimerCoroutine;
        public Stat DamageStat { get; private set; } = new Stat();

        private float durationAttack;
        private float cooldownAttack;
        private bool isReady = true;
        
        public IDamageable Damageable { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            so_Push = (SO_Push)so_Trap;
            pushAnimation = GetComponentInUnit<PushAnimation>();
            durationAttack = Calculate.Attack.TotalDurationInSecond(so_Push.AttackSpeed);
            cooldownAttack = so_Push.CooldownAttack;

            DamageStat.AddValue(so_Push.Damage);
            Damageable = new NormalDamage(gameObject, DamageStat.CurrentValue);
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
        }

        private void OnEnable()
        {
            GetComponentInUnit<PushCollision>().OnHitEnter += OnHitEnter;
        }
        private void OnDisable()
        {
            GetComponentInUnit<PushCollision>().OnHitEnter -= OnHitEnter;
        }

        private void OnHitEnter(GameObject target)
        {
            SetTarget(target);
            ApplyDamage();
        }

        public override void Activate()
        {
            if(!isReady) return;
            isReady = false;
            
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(.5f, AfterActivate));
        }

        private void AfterActivate()
        {
            pushAnimation.ChangeAnimationWithDuration(activateClip, durationAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(durationAttack, Restart));
        }

        private void Restart()
        {
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(1, AfterDeactivate));
        }
        public override void Deactivate()
        {
            
        }

        private void AfterDeactivate()
        {
            pushAnimation.ChangeAnimationWithDuration(deactivateClip, cooldownAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(cooldownAttack, ChangeReady));
        }

        private void ChangeReady()
        {
            this.isReady = !isReady;
        }
        
        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
        } 

        public void ApplyDamage()
        {
            if (CurrentTarget.TryGetComponent(out ITrapAttackable trapAttackable) &&
                CurrentTarget.TryGetComponent(out IHealth health) && 
                health.IsLive)
            {
                trapAttackable.TakeDamage(Damageable);
            }
        }
    }
}
