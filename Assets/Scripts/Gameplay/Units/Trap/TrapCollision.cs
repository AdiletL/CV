using System;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class TrapCollision : MonoBehaviour
    {
        public Action<GameObject> OnHitEnter;
        
        [SerializeField] protected TrapController trapController;

        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(trapController.EnemyLayer, other.gameObject.layer)) return;
            
            OnHitEnter?.Invoke(other.gameObject);
        }
    }
}