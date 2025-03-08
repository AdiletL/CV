using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class PushCollision : TrapCollision
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(!Calculate.GameLayer.IsTarget(trapController.EnemyLayer, collision.gameObject.layer)) return;
            
            OnHitEnter?.Invoke(collision.gameObject);
        }
    }
}