using Unit;
using UnityEngine;

namespace Calculate
{
    public static class Attack
    {
        
        public static float TotalDurationInSecond(float countAttack)
        {
            return 1 / countAttack;
        }
        
        public static GameObject CheckForwardEnemy(GameObject gameObject, Vector3 center, LayerMask layerMask, float distance = .6f)
        {
            RaycastHit[] enemyHits = new RaycastHit[1];
            int hitCount = Physics.RaycastNonAlloc(center,
                gameObject.transform.forward, enemyHits,
                distance, layerMask);

            if (hitCount > 0)
            {
                RaycastHit hit = enemyHits[0];
    
                if (hit.transform.gameObject != gameObject)
                    return hit.transform.gameObject;
            }
            else
            {
                enemyHits[0] = default;
            }

            return null;
        }
        
        public static GameObject FindUnitInRange(Vector3 origin, float radius, LayerMask layerMask, ref Collider[] overlapHits)
        {
            GameObject closestUnit = null;

            int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            if (hitCount == overlapHits.Length)
            {
                overlapHits = new Collider[overlapHits.Length * 2];
                hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            }
            float sqrRadius = radius * radius;
            
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapHits[i];
                
                if (!hit.TryGetComponent(out IHealth health) || !health.IsLive)
                    continue;

                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    var directionToTarget = (unitCenter.Center.position - origin).normalized;
                    float distanceToTargetSqr = (unitCenter.Center.position - origin).sqrMagnitude;
                    
                    if (distanceToTargetSqr <= sqrRadius)
                    {
                        //Debug.DrawRay(origin, directionToTarget * 100, Color.green, 2);
                        if (Physics.Raycast(origin, directionToTarget, out var hit2, radius)
                            && hit2.transform.gameObject == hit.gameObject)
                        {
                            closestUnit = hit.transform.gameObject;
                        }
                    }
                }
            }

            return closestUnit;
        }


        public static bool IsFindUnitInRange(Vector3 origin, float radius, LayerMask layerMask, ref Collider[] overlapHits)
        {
            var isUnit = false;
            float sqrRadius = radius * radius;
            
            int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, overlapHits, layerMask);
            
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapHits[i];
                
                if(!hit.TryGetComponent(out IHealth health) || !health.IsLive) continue;
                
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