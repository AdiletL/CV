using System;
using System.Collections;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Unit.Trap
{
    public class PushController : TrapController, IApplyDamage
    {
        private SO_Push so_Push;
        private PushAnimation pushAnimation;
        private Coroutine startTimerCoroutine;

        private float durationAttack;
        private float cooldownAttack;
        private bool isReady;
        
        public IDamageable Damageable { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            so_Push = (SO_Push)so_Trap;
            pushAnimation = GetComponentInUnit<PushAnimation>();
            durationAttack = Calculate.Attack.TotalDurationInSecond(so_Push.AmountAttackInSecond);
            cooldownAttack = so_Push.CooldownAttack;

            Damageable = new NormalDamage(so_Push.Damage, gameObject);
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
            if(isReady) return;
            isReady = true;
            pushAnimation.ChangeAnimationWithDuration(activateClip, durationAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(durationAttack, PreDeactivate));
        }

        private void PreActivate()
        {
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(1, PreActivate));
        }

        public override void Deactivate()
        {
            isReady = false;
            pushAnimation.ChangeAnimationWithDuration(deactivateClip, cooldownAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(cooldownAttack, PreActivate));
        }

        private void PreDeactivate()
        {
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(1, PreActivate));
        }
        
        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
        } 

        public void ApplyDamage()
        {
            if (CurrentTarget.TryGetComponent(out IHealth health)
                && health.IsLive)
            {
                health.TakeDamage(Damageable);
            }
        }
    }
}
