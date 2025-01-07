using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Damage;
using ScriptableObjects.Gameplay.Trap;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Trap
{
    [RequireComponent(typeof(SphereCollider))]
    public class ThornController : TrapController, IApplyDamage
    {
        private SO_Thorn so_Thorn;
        private ThornAnimation thornAnimation;
        private SphereCollider sphereCollider;

        private Coroutine startTimerCoroutine;
        private Coroutine applyDamageCoroutine;

        private float startTimer;
        private float duration;
        private float applyDamageCooldown;
        private float cooldown;
        private float radius;
        
        private bool isReady = true;
        
        public IDamageable Damageable { get; private set; }
        

        public override void Initialize()
        {
            base.Initialize();
            so_Thorn = (SO_Thorn)so_Trap;
            startTimer = so_Thorn.StartTimer;
            duration = so_Thorn.Duration;
            applyDamageCooldown = so_Thorn.ApplyDamageCooldown;
            cooldown = so_Thorn.Cooldown;
            radius = so_Thorn.Radius;
            EnemyLayers = so_Thorn.EnemyLayers;
            
            Damageable = new NormalDamage(so_Thorn.Damage, gameObject);

            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;

            thornAnimation = components.GetComponentFromArray<ThornAnimation>();
            isReady = true;
        }

        private void Reset()
        {
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            
            if(applyDamageCoroutine != null)
                StopCoroutine(applyDamageCoroutine);
        }
        
        public override void Appear()
        {
            
        }
        
        
        public override void Activate()
        {
            isReady = false;
            Reset();
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(startTimer, Duration));
        }
        public override void Deactivate()
        {
            thornAnimation.ChangeAnimationWithDuration(deactivateClip);
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
            thornAnimation.ChangeAnimationWithDuration(activateClip);
            var interval = activateClip.length;
            
            yield return new WaitForSeconds(interval - .1f);
            while (countTimer < duration)
            {
                affectedEnemies.Clear();

                foreach (var enemyLayer in EnemyLayers)
                {
                    var colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);

                    foreach (var collider in colliders)
                    {
                        if (!affectedEnemies.Contains(collider.gameObject)) // Check if not already affected
                        {
                            affectedEnemies.Add(collider.gameObject);
                            CurrentTarget = collider.gameObject;
                            ApplyDamage();
                        }
                    }
                }

                yield return new WaitForSeconds(applyDamageCooldown);
                countTimer += applyDamageCooldown;
            }

            Deactivate();
        }
        

        public void ApplyDamage()
        {
            if(CurrentTarget.TryGetComponent(out IHealth health) && health.IsLive)
                health.TakeDamage(Damageable);
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            if(!isReady || !Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)) return;
            Activate();
        }
    }
}