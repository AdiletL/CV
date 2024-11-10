using System;
using ScriptableObjects.Character;
using Character;
using UnityEngine;

namespace Character
{
    public abstract class CharacterHealth : MonoBehaviour, IHealth, ICharacter
    {
        public event Action<IHealthInfo> OnChangedHealth;
        
        [SerializeField] protected CharacterMainController characterMainController;
        [SerializeField] protected SO_CharacterHealth so_CharacterHealth;

        private HealthInfo healthInfo;
        private int maxHealth;
        private int currentHealth;
        private bool isLive;

        public virtual int MaxHealth
        {
            get => maxHealth;
            protected set => maxHealth = value; 
        }

        public virtual int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (value <= 0)
                {
                    currentHealth = 0;
                    IsLive = false;
                    gameObject.SetActive(false);
                }
                else
                {
                    currentHealth = value;
                }

                Debug.Log(gameObject.name + " Current Health: " + currentHealth);
                ExecuteEventChangedHealth();
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

        protected virtual HealthInfo CreateHealthInfo()
        {
            return new HealthInfo();
        }

        protected virtual void ExecuteEventChangedHealth()
        {
            healthInfo.CurrentHealth = currentHealth;
            healthInfo.MaxHealth = MaxHealth;
            OnChangedHealth?.Invoke(healthInfo);
        }
        
        public virtual void Initialize()
        {
            healthInfo = CreateHealthInfo();
            MaxHealth = so_CharacterHealth.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        public virtual void IncreaseStates(IState state)
        {
            if (state is CharacterHealthStates characterHealthStates)
            {
                CurrentHealth += characterHealthStates.Health;
                MaxHealth += characterHealthStates.MaxHealth;
            }
        }
        
        public virtual void TakeDamage(IDamageble damageble)
        {
            var totalDamage = damageble.GetTotalDamage(gameObject);
            CurrentHealth -= totalDamage;
        }
    }

    public class HealthInfo : IHealthInfo
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
    }
}