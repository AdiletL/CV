using System;
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
        
        public static GameObject FindUnitInRange(Vector3 origin, float radius, LayerMask layerMask, ref Collider[] overlapHits)
        {
            GameObject closestUnit = null;

            int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            float sqrRadius = radius * radius;
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapHits[i];
                if (hit.TryGetComponent(out UnitCenter unitCenter))
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
            var isUnit = false;
            float sqrRadius = radius * radius;
            
            int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapHits[i];
                
                if(!hit.TryGetComponent(out T component) || 
                   !hit.TryGetComponent(out IHealth health) || !health.IsLive) continue;
                
                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    var directionToTarget = (unitCenter.Center.position - origin).normalized;
                    float distanceToTargetSqr = (unitCenter.Center.position - origin).sqrMagnitude;
                    
                    if (distanceToTargetSqr <= sqrRadius)
                    {
                        //Debug.DrawRay(origin, direcitonToTarget * (radius), Color.green, 2);
                        if (Physics.Raycast(origin, directionToTarget, out var hit2, radius)
                            && hit2.transform.gameObject == hit.gameObject)
                        {
                            isUnit = true;
                            break;
                        }
                    }
                }
            }
            
            for (int i = 0; i < overlapHits.Length; i++)
            {
                overlapHits[i] = null;
            }

            return isUnit;
        }
    }
}