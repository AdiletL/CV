using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Damage;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Trap;
using Unit.Cell;
using UnityEngine;
using Zenject;

namespace Unit.Trap.Fall
{
    public class FallGravityController : TrapController, IFallGravity
    {
        [Inject] private SO_GameConfig gameConfig;
        
        private SO_FallGravity so_FallGravity;
        private Coroutine startTimerCoroutine;
        
        private List<CellController> cellControllers = new();
        
        private float radius;
        private bool isReady = true;
        
        public float Mass { get; private set; }
        public IDamageable Damageable { get; private set; }
        
        
        public override void Initialize()
        {
            base.Initialize();

            so_FallGravity = (SO_FallGravity)so_Trap;
            radius = so_FallGravity.Radius;
            Mass = so_FallGravity.Mass;
            Damageable = new NormalDamage(so_FallGravity.Damage, gameObject);
        }
        
        public override void Appear()
        {
            
        }
        
        public override void Activate()
        {
            if(!isReady) return;
            isReady = false;
            
            FallGravity();
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
                if (colliders[i].TryGetComponent(out IAttackable attackable) &&
                    colliders[i].TryGetComponent(out IHealth health) &&
                    health.IsLive)
                {
                    attackable.TakeDamage(Damageable);
                }
            }
        }

        private void InActiveCells()
        {
            for (int i = cellControllers.Count - 1; i >= 0; i--)
            {
                cellControllers[i].gameObject.SetActive(false);
            }
        }
        
        public void FallGravity()
        {
            CheckCellInRadiusAndAddRigidbody();
        }
        
        private void CheckCellInRadiusAndAddRigidbody()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, Layers.CELL_LAYER);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out CellController cell))
                {
                    cell.gameObject.AddComponent<Rigidbody>();
                    cell.GetComponent<Rigidbody>().mass = Mass;
                    cellControllers.Add(cell);
                }
            }
        }

        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DeathPlane deathPlane))
            {
                ApplyDamage();
                InActiveCells();
                Deactivate();
            }
            
            if(!isReady || 
               !Calculate.GameLayer.IsTarget(EnemyLayer, other.gameObject.layer) || 
               !other.TryGetComponent(out ITrapInteractable trapInteractable)) return;

            if(startTimerCoroutine != null) StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(gameConfig.BaseWaitTimeTrap, Activate));
        }

    }
}