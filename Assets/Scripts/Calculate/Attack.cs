using Unit;
using UnityEngine;

namespace Calculate
{
    public static class Attack
    {
        private static RaycastHit[] enemyHits = new RaycastHit[1];
        
        public static float TotalDurationInSecond(float countAttack)
        {
            return 1 / countAttack;
        }
        
        public static GameObject CheckForwardEnemy(GameObject gameObject, Vector3 center, LayerMask layerMask, float distance = .6f)
        {
            var hitCount = Physics.RaycastNonAlloc(center,
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
        
        public static GameObject FindUnitInRange(Vector3 origin, float distance, int layerMask, ref Collider[] overlapHits)
        {
            GameObject closestUnit = null;
            float closestDistanceSqr = distance;

            int hitCount = Physics.OverlapSphereNonAlloc(origin, distance, overlapHits, layerMask);
            if (hitCount == overlapHits.Length)
            {
                overlapHits = new Collider[overlapHits.Length * 2];
                hitCount = Physics.OverlapSphereNonAlloc(origin, distance, overlapHits, layerMask);
            }

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapHits[i];
                
                if (!hit.TryGetComponent(out IHealth health) || !health.IsLive)
                    continue;

                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    float distanceToTargetSqr = (unitCenter.Center.position - origin).sqrMagnitude;
                    var directionToTarget = (unitCenter.Center.position - origin).normalized;
                    
                    //Debug.DrawRay(origin, direcitonToTarget * 100, Color.red, 2);
                    if (Physics.Raycast(origin, directionToTarget, out var hit2, distance)
                        && hit2.transform.gameObject == hit.gameObject
                        && distanceToTargetSqr <= closestDistanceSqr * closestDistanceSqr)
                    {
                        closestDistanceSqr = Mathf.Sqrt(distanceToTargetSqr);
                        closestUnit = hit.transform.gameObject;
                    }
                }
            }
            
            for (int i = 0; i < overlapHits.Length; i++)
            {
                overlapHits[i] = null;
            }

            return closestUnit;
        }


        public static bool IsFindUnitInRange(Vector3 origin, float distance, int layerMask, Collider[] overlapHits)
        {
            var isUnit = false;
            
            int hitCount = Physics.OverlapSphereNonAlloc(origin, distance, overlapHits, layerMask);
            
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapHits[i];
                
                if(!hit.TryGetComponent(out IHealth health) || !health.IsLive) continue;
                
                if (hit.TryGetComponent(out UnitCenter unitCenter))
                {
                    float distanceToTargetSqr = (unitCenter.Center.position - origin).sqrMagnitude;
                    var direcitonToTarget = (unitCenter.Center.position - origin).normalized;
                    
                    Debug.DrawRay(origin, direcitonToTarget * (distance), Color.red, 2);
                    if (Physics.Raycast(origin, direcitonToTarget, out var hit2, distance) 
                        && hit2.transform.gameObject == hit.gameObject
                        && distanceToTargetSqr <= distance * distance)
                    {
                        isUnit = true;
                        break;
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