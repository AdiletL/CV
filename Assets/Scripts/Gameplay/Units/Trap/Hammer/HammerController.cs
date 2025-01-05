using System;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;
using System.Collections;

namespace Unit.Trap.Hammer
{
    public class HammerController : TrapController, IApplyDamage
    {
        [SerializeField] private Collider hammerCollider;
        
        private SO_Hammer so_Hammer;
        private HammerAnimation hammerAnimation;
        private Coroutine startCooldownCoroutine;
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
            cooldownAttack = Calculate.Attack.TotalDurationInSecond(so_Hammer.CooldownAttack);
            cooldownAttack = so_Hammer.CooldownAttack;
            
            GetComponentInUnit<HammerCollision>()?.Initialize();
            
            Activate();
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
            if(!isReady) return;
            CurrentTarget = target;
            ApplyDamage();
        }

        private void OnHitExit(GameObject target)
        {
            if(CurrentTarget == null || target != CurrentTarget) return;
            hammerCollider.isTrigger = false;
        }

        public override void Activate()
        {
            if(isReady) return;
            
            isReady = true;
            hammerCollider.isTrigger = true;
            hammerAnimation.ChangeAnimationWithDuration(activateClip, durationAttack);
            if(startCooldownCoroutine != null)
                StopCoroutine(startCooldownCoroutine);
            StartCoroutine(StartCooldownCoroutine(durationAttack, AfterActivate));
        }

        private void AfterActivate()
        {
            int colliderCount = 0;
            foreach (var VARIABLE in EnemyLayers)
            {
                colliderCount = Physics.OverlapSphereNonAlloc(transform.position, Platform.Radius, checkUnitColliders, VARIABLE);
                if(colliderCount > 0) break;
            }
            
            if(colliderCount == 0)
                hammerCollider.isTrigger = false;
            
            if(startCooldownCoroutine != null)
                StopCoroutine(startCooldownCoroutine);
            StartCoroutine(StartCooldownCoroutine(1, Deactivate));
        }

        public override void Deactivate()
        {
            if(!isReady) return;
            
            isReady = false;
            hammerAnimation.ChangeAnimationWithDuration(deactivateClip, cooldownAttack);
            if(startCooldownCoroutine != null)
                StopCoroutine(startCooldownCoroutine);
            StartCoroutine(StartCooldownCoroutine(cooldownAttack, AfterDeactivate));
        }

        private void AfterDeactivate()
        {
            if(startCooldownCoroutine != null)
                StopCoroutine(startCooldownCoroutine);
            StartCoroutine(StartCooldownCoroutine(1, Activate));
        }

        private IEnumerator StartCooldownCoroutine(float interval, Action action)
        {
            yield return new WaitForSeconds(interval);
            action?.Invoke();
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