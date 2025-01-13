using UnityEngine;

namespace Gameplay.Damage
{
    public class NormalDamage : IDamageable
    {
        public GameObject Owner { get; set; }
        public int Amount { get; }
        public int AdditionalDamage { get; private set; }
        
        public void AddAdditionalDamage(int value)
        {
            AdditionalDamage += value;
        }
        public void RemoveAdditionalDamage(int value)
        {
            AdditionalDamage -= value;
        }

        public NormalDamage(int amount, GameObject gameObject)
        {
            this.Amount = amount;
            this.Owner = gameObject;

            if (Amount < 0) Amount = 999999;
        }
        
        public int GetTotalDamage(GameObject gameObject)
        {
            var result = 0;
            result = Amount + AdditionalDamage;
            return result;
        }
    }
}