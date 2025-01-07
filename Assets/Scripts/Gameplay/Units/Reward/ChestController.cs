using System;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Reward
{
    public class ChestController : RewardController
    {
        public event Action OnChestOpen;

        
        public override void Appear()
        {
            
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                OnChestOpen?.Invoke();
            }
        }
    }
}
