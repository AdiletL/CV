using UnityEngine;

namespace Gameplay.Damage
{
    public class NormalDamage : IDamageble
    {
        public int Amount { get; set; }

        public NormalDamage(int amount)
        {
            this.Amount = amount;
        }
        
        public int GetTotalDamage(GameObject gameObject)
        {
            var result = 0;
            result = Amount;
            return result;
        }
    }
}