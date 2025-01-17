using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Damage;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Trap;
using Unit.Cell;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Unit.Trap.Fall
{
    public class FallController : TrapController, IFall, IApplyDamage
    {
        private SO_Fall so_Fall;
        private SO_GameConfig gameConfig;
        
        private Coroutine startTimerCoroutine;
        private Coroutine fallUpdateCoroutine;
        
        private List<UnitController> cellControllers = new();
        
        private float radius;
        private bool isReady = true;
        
        
        public IDamageable Damageable { get; private set; }
        public float Speed { get; private set; }


        [Inject]
        private void Construct(SO_GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        public override void Initialize()
        {
            base.Initialize();

            so_Fall = (SO_Fall)so_Trap;
            Speed = so_Fall.Speed;
            radius = so_Fall.Radius;
            Damageable = new NormalDamage(so_Fall.Damage, gameObject);
        }

        public override void Appear()
        {
            
        }
        
        
        public override void Activate()
        {
            if(!isReady) return;
            isReady = false;
            
            CheckUnitInRadius();
            Fall();
        }

        private void CheckUnitInRadius()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, Layers.CELL_LAYER);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out CellController cell))
                {
                    cellControllers.Add(cell);
                }
            }
        }
        
        public override void Deactivate()
        {
            if(isReady) return;
            isReady = true;
        }

        public void ApplyDamage()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, ~Layers.CELL_LAYER);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out IHealth health)
                    && health.IsLive)
                {
                    health.TakeDamage(Damageable);
                }
            }
        }
        
        public void Fall()
        {
            if(fallUpdateCoroutine != null) StopCoroutine(fallUpdateCoroutine);
            fallUpdateCoroutine = StartCoroutine(FallUpdateCoroutine());
        }

        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
        }

        private IEnumerator FallUpdateCoroutine()
        {
            float timer = 30;
            float countTimer = 0;
            float deltaTime = Speed * Time.deltaTime;
            while (timer > countTimer)
            {
                yield return null;
                for (int i = cellControllers.Count - 1; i >= 0; i--)
                {
                    if (!cellControllers[i].gameObject.activeSelf)
                    {
                        cellControllers.RemoveAt(i);
                        continue;
                    }

                    cellControllers[i].MoveDirection(Vector3.down, deltaTime);
                    countTimer += Time.deltaTime;
                }
            }

            ApplyDamage();
            Deactivate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DeathPlane deathPlane))
            {
                StopCoroutine(fallUpdateCoroutine);
                ApplyDamage();
                Deactivate();
            }
            
            if(!isReady || !Calculate.GameLayer.IsTarget(EnemyLayer, other.gameObject.layer)) return;

            if(startTimerCoroutine != null) StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(gameConfig.BaseWaitTimeTrap, Activate));
        }

    }
}