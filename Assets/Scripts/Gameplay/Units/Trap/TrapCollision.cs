using System;
using UnityEngine;

namespace Unit.Trap
{
    public class TrapCollision : MonoBehaviour
    {
        public Action<GameObject> OnHitEnter;
        
        [SerializeField] protected TrapController trapController;

        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(trapController.EnemyLayers, other.gameObject.layer)) return;
            
            OnHitEnter?.Invoke(other.gameObject);
        }
    }
}