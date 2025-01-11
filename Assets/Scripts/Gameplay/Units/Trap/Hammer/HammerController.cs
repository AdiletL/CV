using System;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;
using System.Collections;
using Unit.Cell;
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
            durationAttack = Calculate.Attack.TotalDurationInSecond(so_Hammer.AmountAttackInSecond);
            cooldownAttack = so_Hammer.CooldownAttack;
            
            GetComponentInUnit<HammerCollision>()?.Initialize();

        }
        
        public override void Appear()
        {
            int random = Random.Range(0, 4);
            Invoke(nameof(Deactivate), random);
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
            SetTarget(target);
            ApplyDamage();
        }

        private void OnHitExit(GameObject target)
        {
            if(CurrentTarget == null || target != CurrentTarget) return;
            hammerCollider.isTrigger = false;
        }

        public override void Activate()
        {
            isReady = false;
            hammerCollider.isTrigger = true;
            hammerAnimation.ChangeAnimationWithDuration(activateClip, durationAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(durationAttack, AfterActivate));
        }

        private void AfterActivate()
        {
            int colliderCount = 0;
            colliderCount = Physics.OverlapSphereNonAlloc(transform.position, CellController.Radius, checkUnitColliders, EnemyLayer);
            
            if(colliderCount == 0)
                hammerCollider.isTrigger = false;
            
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(1, Deactivate));
        }

        public override void Deactivate()
        {
            isReady = true;
            hammerAnimation.ChangeAnimationWithDuration(deactivateClip, cooldownAttack);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(cooldownAttack, AfterDeactivate));
        }

        private void AfterDeactivate()
        {
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
            if (CurrentTarget.TryGetComponent(out IHealth health)
                && health.IsLive)
            {
                health.TakeDamage(Damageable);
            }
        }
    }
}