using System;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Trap
{
    public class AxeCollision : MonoBehaviour
    {
        public event Action<GameObject> OnHit;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController playerController))
            {
                OnHit?.Invoke(playerController.gameObject);
            }
        }
    }
}
