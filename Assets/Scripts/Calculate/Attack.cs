using UnityEngine;

namespace Calculate
{
    public static class Attack
    {
        private static readonly Vector3 originRayForward = Vector3.up * .5f;
        private static RaycastHit[] enemyHits = new RaycastHit[1];
        
        public static float TotalDurationAttack(float countAttack)
        {
            return 1 / countAttack;
        }
        
        public static GameObject CheckForwardEnemy(GameObject gameObject)
        {
            var hitCount = Physics.RaycastNonAlloc(gameObject.transform.position + originRayForward,
                gameObject.transform.forward, enemyHits,
                .6f, Layers.ENEMY_LAYER);

            // Если был хотя бы один результат
            if (hitCount > 0)
            {
                RaycastHit hit = enemyHits[0];
    
                // Проверяем, что найденный объект не совпадает с текущим GameObject
                if (hit.transform.gameObject != gameObject)
                    return hit.transform.gameObject;
            }

            return null;
        }
    }
}