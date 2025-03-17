using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Unit.Cell;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Trap.Fall
{
    public class FallGravityController : TrapController, IFallGravity
    {
        [Inject] private SO_GameConfig gameConfig;
        
        private SO_FallGravity so_FallGravity;
        private Coroutine startTimerCoroutine;
        private Coroutine checkAndAddRigidBodyCoroutine;
        public Stat DamageStat { get; private set; } = new Stat();
        
        private float intervalFallObjects;
        private float radius;
        private bool isReady = true;
        
        private List<CellController> cellControllers = new();
        
        public DamageData DamageData { get; private set; }
        public float Mass { get; private set; }


        public override void Initialize()
        {
            base.Initialize();

            DamageStat.AddCurrentValue(so_FallGravity.Damage);
            so_FallGravity = (SO_FallGravity)so_Trap;
            Mass = so_FallGravity.Mass + Physics.gravity.y;
            radius = so_FallGravity.Radius + gameConfig.RadiusCell;
            intervalFallObjects = so_FallGravity.IntervalFallObjets;
            DamageData = new DamageData(gameObject, DamageType.Physical, DamageStat.CurrentValue);
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
            if(!isReady) return;
            isReady = false;
            
            FallGravity();
        }

        public override void Reset()
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
                    attackable.TakeDamage(DamageData);
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
            if(checkAndAddRigidBodyCoroutine != null) StopCoroutine(checkAndAddRigidBodyCoroutine);
            checkAndAddRigidBodyCoroutine = StartCoroutine(CheckCellInRadiusAndAddRigidbodyCoroutine());
        }
        
        private IEnumerator CheckCellInRadiusAndAddRigidbodyCoroutine()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, Layers.CELL_LAYER);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i].TryGetComponent(out CellController cell))
                {
                    if(cell.IsBlocked()) continue;
                    if(!cell.gameObject.TryGetComponent(out Rigidbody rigidBody))
                        cell.gameObject.AddComponent<Rigidbody>();
                    
                    cell.GetComponent<Rigidbody>().mass = Mass;
                    cellControllers.Add(cell);
                    yield return new WaitForSeconds(intervalFallObjects);
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