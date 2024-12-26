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

        private SphereCollider sphereCollider;
        private Animator animator;

        private Coroutine timerCoroutine;
        private Coroutine applyDamageCoroutine;

        private GameObject target;
        private LayerMask[] enemyLayers;

        private float startTimer;
        private float duration;
        private float applyDamageCooldown;
        private float cooldown;
        private float radius;

        private bool isReady;
        
        private const string ACTIVATE_NAME = "Activate";
        private const string DEACTIVATE_NAME = "Deactivate";
        
        public IDamageable Damageable { get; private set; }

        public override T GetComponentInUnit<T>()
        {
            return GetComponent<T>();
        }
        

        public override void Initialize()
        {
            base.Initialize();
            so_Thorn = (SO_Thorn)so_Trap;
            startTimer = so_Thorn.StartTimer;
            duration = so_Thorn.Duration;
            applyDamageCooldown = so_Thorn.ApplyDamageCooldown;
            cooldown = so_Thorn.Cooldown;
            radius = so_Thorn.Radius;
            enemyLayers = so_Thorn.EnemyLayers;
            
            Damageable = new NormalDamage(so_Thorn.Damage, gameObject);

            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;
            
            animator = GetComponent<Animator>();

            isReady = true;
        }

        private void Reset()
        {
            if(timerCoroutine != null)
                StopCoroutine(timerCoroutine);
            
            if(applyDamageCoroutine != null)
                StopCoroutine(applyDamageCoroutine);
        }
        public override void Activate()
        {
            if(!isReady) return;
            
            isReady = false;
            Reset();
            timerCoroutine = StartCoroutine(StartCooldownCoroutine(startTimer, Duration));
        }
        public override void Deactivate()
        {
            if(isReady) return;
            
            animator.SetTrigger(DEACTIVATE_NAME);
            if(timerCoroutine != null)
                StopCoroutine(timerCoroutine);
                
            timerCoroutine = StartCoroutine(StartCooldownCoroutine(cooldown, () =>
            {
                isReady = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, Layers.PLAYER_LAYER);
                if (colliders.Length > 0)
                {
                    Activate();
                }
            }));
        }
        
        private IEnumerator StartCooldownCoroutine(float timer, Action action)
        {
            float countTimer = 0;
            while (countTimer < timer)
            {
                countTimer += Time.deltaTime;
                yield return null;
            }
            
            action?.Invoke();
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
            animator.SetTrigger(ACTIVATE_NAME);

            while (countTimer < duration)
            {
                affectedEnemies.Clear();

                foreach (var enemyLayer in enemyLayers)
                {
                    var colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);

                    foreach (var collider in colliders)
                    {
                        if (!affectedEnemies.Contains(collider.gameObject)) // Check if not already affected
                        {
                            affectedEnemies.Add(collider.gameObject);
                            target = collider.gameObject;
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
            if(target.TryGetComponent(out IHealth health) && health.IsLive)
                health.TakeDamage(Damageable);
        }

        private void ApplyDamage(IHealth health)
        {
            if(health.IsLive)
                health.TakeDamage(Damageable);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                if (!isReady)
                {
                    if (player.TryGetComponent(out IHealth health))
                    {
                        ApplyDamage(health);
                    }
                }
                else
                {
                    Activate();
                }
            }
        }
    }
}