using UnityEngine;

namespace Gameplay.Damage
{
    public class NormalDamage : IDamageble
    {
        public int amount { get; set; }

        public NormalDamage(int amount)
        {
            this.amount = amount;
        }
        
        public int GetTotalDamage(GameObject gameObject)
        {
            var result = 0;
            result = amount;
            return result;
        }
    }
}