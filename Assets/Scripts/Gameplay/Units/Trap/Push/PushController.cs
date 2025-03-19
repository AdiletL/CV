using System;
using System.Collections;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class PushController : TrapController, IApplyDamage
    {
        private SO_Push so_Push;
        private Coroutine startTimerCoroutine;
        public Stat DamageStat { get; private set; } = new Stat();

        private float durationAttack;
        private float cooldownAttack;
        private bool isReady = true;
        
        public DamageData DamageData { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            so_Push = (SO_Push)so_Trap;
            Calculate.Convert.AttackSpeedToDuration(so_Push.AttackSpeed);
            cooldownAttack = so_Push.CooldownAttack;

            DamageStat.AddCurrentValue(so_Push.Damage);
            DamageData = new DamageData(gameObject, DamageType.Physical, DamageStat.CurrentValue);
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
            //GetComponentInUnit<PushCollision>().OnHitEnter += OnHitEnter;
        }
        private void OnDisable()
        {
            //GetComponentInUnit<PushCollision>().OnHitEnter -= OnHitEnter;
        }

        private void OnHitEnter(GameObject target)
        {
            //SetTarget(target);
            ApplyDamage();
        }

        public void Trigger()
        {
            if(!isReady) return;
            isReady = false;
            
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(.5f, AfterActivate));
        }

        private void AfterActivate()
        {
            //pushAnimation.ChangeAnimationWithDuration(playClip, durationAttack);
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
        private void AfterDeactivate()
        {
           // pushAnimation.ChangeAnimationWithDuration(resetClip, cooldownAttack);
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
            
        }
    }
}
