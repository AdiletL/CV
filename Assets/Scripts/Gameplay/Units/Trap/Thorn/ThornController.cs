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
        [SerializeField] private GameObject asdf;
        
        private SO_Thorn so_Thorn;

        private SphereCollider sphereCollider;

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
        
        public IDamageable Damageable { get; private set; }

        public override T GetComponentInUnit<T>()
        {
            return GetComponent<T>();
        }


        private void Start()
        {
            Initialize();
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

            isReady = true;
        }

        public override void Activate()
        {
            isReady = false;
            
            asdf.SetActive(false);
            if(timerCoroutine != null)
                StopCoroutine(timerCoroutine);
                
            timerCoroutine = StartCoroutine(TimerCoroutine(startTimer, Duration));
        }
        public override void Deactivate()
        {
            if(timerCoroutine != null)
                StopCoroutine(timerCoroutine);
                
            timerCoroutine = StartCoroutine(TimerCoroutine(cooldown, () =>
            {
                isReady = true;
                asdf.SetActive(true);
            }));
        }
        
        private IEnumerator TimerCoroutine(float timer, Action action)
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