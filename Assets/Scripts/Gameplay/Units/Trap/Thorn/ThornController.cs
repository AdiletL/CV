using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.Gameplay.Trap;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class ThornController : TrapController, IApplyDamage
    {
        private SO_Thorn so_Thorn;

        private Coroutine startIntervalResetActionCoroutine;
        
        private float radius;
        private const float INTERVAL_RESET_ACTION = 1.5f;
        
        public DamageData DamageData { get; private set; }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private List<GameObject> FindUnitsInRange()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius, EnemyLayer);
            if (colliders.Length > 0)
            {
                var list = new List<GameObject>();
                for (int i = colliders.Length - 1; i >= 0; i--)
                {
                    var target = colliders[i]?.gameObject;
                    if (!target) continue;

                    list.Add(target);
                }

                return list;
            }

            return null;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            so_Thorn = (SO_Thorn)so_Trap;
            cooldown = so_Thorn.Cooldown;
            EnemyLayer = so_Thorn.EnemyLayer;

            var trapCollision = GetComponentInUnit<TrapCollision>();
            if(trapCollision && trapCollision.TryGetComponent(out SphereCollider sphereCollider))
                radius = sphereCollider.radius;
            
            DamageData = new DamageData(gameObject, DamageType.Physical, so_Thorn.Damage, false);
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
           
        }

        public void ApplyDamage()
        {
            if(startIntervalResetActionCoroutine != null) StopCoroutine(startIntervalResetActionCoroutine);
            startIntervalResetActionCoroutine = StartCoroutine(StartIntervalResetActionCoroutine());
            
            var targets = FindUnitsInRange();
            if (targets == null) return;

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i] &&
                    targets[i].TryGetComponent(out ITrapAttackable trapAttackable) &&
                    targets[i].TryGetComponent(out IHealth health) &&
                    health.IsLive)
                {
                    trapAttackable.TakeDamage(DamageData);
                }
            }
        }

        private IEnumerator StartIntervalResetActionCoroutine()
        {
            yield return new WaitForSeconds(INTERVAL_RESET_ACTION);
            ResetAction();
        }

    }
}