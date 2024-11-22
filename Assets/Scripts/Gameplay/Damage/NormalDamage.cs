using UnityEngine;

namespace Gameplay.Damage
{
    public class NormalDamage : IDamageable
    {
        public GameObject GameObject { get; set; }
        public int Amount { get; set; }

        public NormalDamage(int amount, GameObject gameObject)
        {
            this.Amount = amount;
            this.GameObject = gameObject;
        }
        
        public int GetTotalDamage(GameObject gameObject)
        {
            var result = 0;
            result = Amount;
            return result;
        }
    }
}