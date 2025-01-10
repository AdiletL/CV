using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Damage;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Trap;
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
        
        private List<UnitController> targetUnits = new();
        
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
            var colliders = Physics.OverlapSphere(transform.position, radius, ~Layers.PLAYER_LAYER);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                UnitController unit = null;
                if(colliders[i].TryGetComponent(out FallController fallController)
                   && fallController == this)
                    continue;
                
                if (colliders[i].TryGetComponent(out UnitController unitController))
                {
                    unit = unitController;
                    if (!unit && unit.TryGetComponent(out UnitCollision unitCollision))
                        unit = unitCollision.UnitController;
                }
                
                if(unit && unitController.UnitType != UnitType.environment) 
                    targetUnits.Add(unit);
            }
        }
        
        public override void Deactivate()
        {
            if(isReady) return;
            isReady = true;
        }

        public void ApplyDamage()
        {
            for (int i = targetUnits.Count - 1; i >= 0; i--)
            {
                if (targetUnits[i].gameObject.activeSelf)
                {
                    if(targetUnits[i].TryGetComponent(out IHealth health)
                       && health.IsLive)
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
            float distance = 500;
            var startPos = transform.position;
            while ((startPos - transform.position).sqrMagnitude < distance)
            {
                yield return null;
                for (int i = targetUnits.Count - 1; i >= 0; i--)
                {
                    if (!targetUnits[i].gameObject.activeSelf)
                    {
                        targetUnits.RemoveAt(i);
                        continue;
                    }

                    targetUnits[i].MoveDirection(Vector3.down, Speed * Time.deltaTime);
                }
            }

            ApplyDamage();
            Deactivate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isReady || !Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)) return;

            if(startTimerCoroutine != null) StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(gameConfig.BaseWaitTimeTrap, Activate));
        }

    }
}