using System;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace Gameplay.Unit.Trap.Hammer
{
    public class HammerController : TrapController, IApplyDamage
    {
        [SerializeField] private Collider hammerCollider;
        
        private SO_Hammer so_Hammer;
        private Coroutine startTimerCoroutine;
        private Collider[] checkUnitColliders = new Collider[1];
        public Stat DamageStat { get; private set; } = new Stat();

        private float durationAttack;
        private float cooldownAttack;
        private bool isReady;
        
        public DamageData DamageData { get; private set; }
        

        public override void Initialize()
        {
            base.Initialize();
            so_Hammer = (SO_Hammer)so_Trap;
            DamageStat.AddCurrentValue(so_Hammer.Damage);
            DamageData = new DamageData(gameObject, DamageType.Physical, DamageStat.CurrentValue, false);
            durationAttack = Calculate.Convert.AttackSpeedToDuration(so_Hammer.AttackSpeed);
            cooldownAttack = so_Hammer.CooldownAttack;
        }
        
        public override void Appear()
        {
            int random = Random.Range(0, 4);
            //Invoke(nameof(Deactivate), random);
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
        }


        private void OnEnable()
        {
            //GetComponentInUnit<HammerCollision>().OnHitEnter += OnHitEnter;
            //GetComponentInUnit<HammerCollision>().OnHitExit += OnHitExit;
        }

        private void OnDisable()
        {
            //GetComponentInUnit<HammerCollision>().OnHitEnter -= OnHitEnter;
            //GetComponentInUnit<HammerCollision>().OnHitExit -= OnHitExit;
        }

        private void OnHitEnter(GameObject target)
        {
            if(isReady) return;

            hammerCollider.isTrigger = true;
            //SetTarget(target);
            ApplyDamage();
        }
        
        private void OnHitExit(GameObject target)
        {
            if(!isReady) return;

            hammerCollider.isTrigger = false;
            //SetTarget(null);
        }
        
        public void Trigger()
        {
            isReady = false;
            
           // if(!CurrentTarget)
            //    hammerCollider.isTrigger = false;
            
            //hammerAnimation.ChangeAnimationWithDuration(playClip, durationAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(durationAttack, AfterActivate));
        }

        private void AfterActivate()
        {
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(1, Deactivate));
        }

        public void Reset()
        {
            //hammerAnimation.ChangeAnimationWithDuration(resetClip, cooldownAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(cooldownAttack, AfterDeactivate));
        }

        private void AfterDeactivate()
        {
            isReady = true;
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(1, Activate));
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