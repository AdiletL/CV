using UnityEngine;

namespace Gameplay.Damage
{
    public class NormalDamage : IDamageable
    {
        public GameObject Owner { get; set; }
        public int CurrentDamage { get; }
        public int AdditionalDamage { get; private set; }
        
        public NormalDamage(int amount, GameObject gameObject)
        {
            this.CurrentDamage = amount;
            this.Owner = gameObject;

            if (CurrentDamage < 0) CurrentDamage = 999999;
        }
        
        public void AddAdditionalDamage(int value)
        {
            AdditionalDamage += value;
        }
        public void RemoveAdditionalDamage(int value)
        {
            AdditionalDamage -= value;
        }
        
        public int GetTotalDamage(GameObject gameObject)
        {
            var result = 0;
            result = CurrentDamage + AdditionalDamage;
            return result;
        }
    }
}