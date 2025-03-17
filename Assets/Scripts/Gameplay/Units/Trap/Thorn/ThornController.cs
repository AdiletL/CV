using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    [RequireComponent(typeof(SphereCollider))]
    public class ThornController : TrapController, IApplyDamage
    {
        private SO_Thorn so_Thorn;
        private SphereCollider sphereCollider;
        public Stat DamageStat { get; private set; } = new Stat();

        private Coroutine startTimerCoroutine;
        private Coroutine applyDamageCoroutine;

        private float startTimer;
        private float duration;
        private float applyDamageCooldown;
        private float cooldown;
        private float radius;
        
        private bool isReady;
        
        public DamageData DamageData { get; private set; }


        public override void Initialize()
        {
            base.Initialize();
            so_Thorn = (SO_Thorn)so_Trap;
            startTimer = so_Thorn.StartTimer;
            duration = so_Thorn.Duration;
            applyDamageCooldown = so_Thorn.ApplyDamageCooldown;
            cooldown = so_Thorn.Cooldown;
            radius = so_Thorn.Radius;
            EnemyLayer = so_Thorn.EnemyLayer;
            
            DamageStat.AddCurrentValue(so_Thorn.Damage);
            DamageData = new DamageData(gameObject, DamageType.Physical, DamageStat.CurrentValue);

            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;

            isReady = true;
        }

        private void Stop()
        {
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            
            if(applyDamageCoroutine != null)
                StopCoroutine(applyDamageCoroutine);
        }
        
        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new NotImplementedException();
        }


        public override void Trigger()
        {
            isReady = false;
            Reset();
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(startTimer, Duration));
        }
        public override void Reset()
        {
            trapAnimation.ChangeAnimationWithDuration(deappearClip);
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
                
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(cooldown, () =>
            {
                isReady = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, Layers.PLAYER_LAYER);
                if (colliders.Length > 0)
                {
                    Activate();
                }
            }));
        }
        
        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            float countTimer = 0;
            while (countTimer < waitTime)
            {
                countTimer += Time.deltaTime;
                yield return null;
            }
            
            callback?.Invoke();
        }

        private void Duration()
        {
            if(applyDamageCoroutine != null)
                StopCoroutine(applyDamageCoroutine);
            
            applyDamageCoroutine = StartCoroutine(ApplyDamageCoroutine());
        }

        private IEnumerator ApplyDamageCoroutine()
        {
            float countTimer = 0;

            HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();
            trapAnimation.ChangeAnimationWithDuration(appearClip);
            var interval = appearClip.length;
            
            yield return new WaitForSeconds(interval - .1f);
            while (countTimer < duration)
            {
                affectedEnemies.Clear();

                var colliders = Physics.OverlapSphere(transform.position, radius, EnemyLayer);

                foreach (var collider in colliders)
                {
                    if (!affectedEnemies.Contains(collider.gameObject)) // Check if not already affected
                    {
                        affectedEnemies.Add(collider.gameObject);
                        CurrentTarget = collider.gameObject;
                        ApplyDamage();
                    }
                }

                yield return new WaitForSeconds(applyDamageCooldown);
                countTimer += applyDamageCooldown;
            }

            Deactivate();
        }
        

        public void ApplyDamage()
        {
            if(CurrentTarget.TryGetComponent(out ITrapAttackable trapAttackable) &&
               CurrentTarget.TryGetComponent(out IHealth health) && 
               health.IsLive)
                trapAttackable.TakeDamage(DamageData);
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            if(!isReady || !Calculate.GameLayer.IsTarget(EnemyLayer, other.gameObject.layer)) return;
            Activate();
        }
    }
}