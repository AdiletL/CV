using ScriptableObjects.Character;
using UnityEngine;

namespace Character
{
    public abstract class CharacterHealth : MonoBehaviour, IHealth
    {
        [SerializeField] protected SO_CharacterHealth so_CharacterHealth;


        private int currentHealth;
        private bool isLive;
        
        public int MaxHealth { get; set; }

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (value <= 0)
                {
                    currentHealth = 0;
                    IsLive = false;
                }
                else
                {
                    currentHealth = value;
                }
                Debug.Log(gameObject.name + " / " + currentHealth);
            }
        }

        public bool IsLive
        {
            get => isLive;
            set
            {
                if (CurrentHealth <= 0)
                    isLive = false;
                else
                    isLive = value;
            }
        }

        public virtual void Initialize()
        {
            MaxHealth = so_CharacterHealth.MaxHealth;
            CurrentHealth = MaxHealth;
        }
        public virtual void TakeDamage(IDamageble damageble)
        {
            var totalDamage = damageble.GetTotalDamage(gameObject);
            CurrentHealth -= totalDamage;
        }
    }
}