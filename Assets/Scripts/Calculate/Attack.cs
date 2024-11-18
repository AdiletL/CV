using UnityEngine;

namespace Calculate
{
    public static class Attack
    {
        private static readonly Vector3 originRayForward = Vector3.up * .25f;
        private static RaycastHit[] enemyHits = new RaycastHit[1];
        
        public static float TotalDurationAttack(float countAttack)
        {
            return 1 / countAttack;
        }
        
        public static GameObject CheckForwardEnemy(GameObject gameObject, int layerMask, float distance = .6f)
        {
            var hitCount = Physics.RaycastNonAlloc(gameObject.transform.position + originRayForward,
                gameObject.transform.forward, enemyHits,
                distance, layerMask);

            if (hitCount > 0)
            {
                RaycastHit hit = enemyHits[0];
    
                if (hit.transform.gameObject != gameObject)
                    return hit.transform.gameObject;
            }

            return null;
        }
    }
}