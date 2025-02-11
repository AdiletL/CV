using System;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace Unit.Trap.Hammer
{
    public class HammerController : TrapController, IApplyDamage
    {
        [SerializeField] private Collider hammerCollider;
        
        private SO_Hammer so_Hammer;
        private HammerAnimation hammerAnimation;
        private Coroutine startTimerCoroutine;
        private Collider[] checkUnitColliders = new Collider[1];

        private float durationAttack;
        private float cooldownAttack;
        private bool isReady;
        
        public IDamageable Damageable { get; private set; }
        

        public override void Initialize()
        {
            base.Initialize();
            hammerAnimation = components.GetComponentFromArray<HammerAnimation>();
            so_Hammer = (SO_Hammer)so_Trap;
            Damageable = new NormalDamage(so_Hammer.Damage, gameObject);
            durationAttack = Calculate.Attack.TotalDurationInSecond(so_Hammer.AttackSpeed);
            cooldownAttack = so_Hammer.CooldownAttack;
            
            GetComponentInUnit<HammerCollision>()?.Initialize();

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
            GetComponentInUnit<HammerCollision>().OnHitEnter += OnHitEnter;
            GetComponentInUnit<HammerCollision>().OnHitExit += OnHitExit;
        }

        private void OnDisable()
        {
            GetComponentInUnit<HammerCollision>().OnHitEnter -= OnHitEnter;
            GetComponentInUnit<HammerCollision>().OnHitExit -= OnHitExit;
        }

        private void OnHitEnter(GameObject target)
        {
            if(isReady) return;

            hammerCollider.isTrigger = true;
            SetTarget(target);
            ApplyDamage();
        }
        
        private void OnHitExit(GameObject target)
        {
            if(!isReady) return;

            hammerCollider.isTrigger = false;
            SetTarget(null);
        }
        
        public override void Activate()
        {
            isReady = false;
            
            if(!CurrentTarget)
                hammerCollider.isTrigger = false;
            
            hammerAnimation.ChangeAnimationWithDuration(activateClip, durationAttack);
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

        public override void Deactivate()
        {
            hammerAnimation.ChangeAnimationWithDuration(deactivateClip, cooldownAttack);
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
            if (CurrentTarget.TryGetComponent(out ITrapAttackable trapAttackable) &&
                CurrentTarget.TryGetComponent(out IHealth health) && 
                health.IsLive)
            {
                trapAttackable.TakeDamage(Damageable);
            }
        }
    }
}