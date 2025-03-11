using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class PushTrigger : TrapTrigger
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(!Calculate.GameLayer.IsTarget(trapController.EnemyLayer, collision.gameObject.layer)) return;
            
            OnHitEnter?.Invoke(collision.gameObject);
        }
    }
}