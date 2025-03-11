using System;
using Gameplay.Unit;
using Unit;
using UnityEngine;

namespace Calculate
{
    public static class Attack
    {
        public static float TotalDurationInSecond(float attackSpeed)
        {
            return 100 / attackSpeed;
        }
        
        public static GameObject FindUnitInRange<T>(Vector3 origin, float radius, LayerMask layerMask, ref Collider[] overlapHits)
        {
            GameObject closestUnit = null;

            int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            float sqrRadius = radius * radius;
            for (int i = hitCount - 1; i >= 0; i--)
            {
                Collider hit = overlapHits[i];
                if (hit.TryGetComponent(out T component) && 
                    hit.TryGetComponent(out IHealth health) &&
                    health.IsLive &&
                    hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    float distanceToTargetSqr = (unitCenter.Center.position - origin).sqrMagnitude;
                    if (distanceToTargetSqr <= sqrRadius)
                        closestUnit = hit.transform.gameObject;
                }
            }

            return closestUnit;
        }


        public static bool IsFindUnitInRange<T>(Vector3 origin, float radius, LayerMask layerMask, ref Collider[] overlapHits)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            float sqrRadius = radius * radius;
            for (int i = hitCount - 1; i >= 0; i--)
            {
                Collider hit = overlapHits[i];
                
                if(!hit.TryGetComponent(out T component) || 
                   !hit.TryGetComponent(out IHealth health) || !health.IsLive) continue;
                
                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    float distanceToTargetSqr = (unitCenter.Center.position - origin).sqrMagnitude;
                    if (distanceToTargetSqr <= sqrRadius)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}