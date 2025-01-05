using UnityEngine;

namespace Unit.Trap
{
    public class PushCollision : TrapCollision
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(!Calculate.GameLayer.IsTarget(trapController.EnemyLayers, collision.gameObject.layer)) return;
            
            OnHitEnter?.Invoke(collision.gameObject);
        }
    }
}