using System;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public class TrapCollision : MonoBehaviour
    {
        public event Action<GameObject> OnHit;
        
        [SerializeField] protected TrapController trapController;
        
        private void OnTriggerEnter(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(trapController.EnemyLayer, other.gameObject.layer)) return;
            OnHit?.Invoke(other.gameObject);
            trapController.StartAction();
        }
    }
}