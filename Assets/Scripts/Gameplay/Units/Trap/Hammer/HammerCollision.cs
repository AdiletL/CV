using System;
using UnityEngine;

namespace Unit.Trap.Hammer
{
    public class HammerCollision : TrapCollision
    {
        public event Action<GameObject> OnHitExit;
        
        private HammerController hammerController;

        public void Initialize()
        {
            hammerController = (HammerController)trapController;
        }

        private void OnTriggerExit(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(hammerController.EnemyLayers, other.gameObject.layer)) return;
            
            OnHitExit?.Invoke(other.gameObject);
        }
    }
}