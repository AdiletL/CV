using System;
using ScriptableObjects.Unit;
using UnityEngine;

namespace Unit
{
    public abstract class UnitHealth : MonoBehaviour, IHealth
    {
        public event Action<HealthInfo> OnChangedHealth;
        public event Action OnDeath;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitHealth so_UnitHealth;
        
        private HealthInfo healthInfo;
        private int maxHealth;
        private int currentHealth;
        private bool isLive;
        
        public GameObject Damaging { get; private set; }

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
                    IsLive = true;
                }

                //Debug.Log(gameObject.name + " Current Health: " + currentHealth);
                ExecuteEventChangedHealth();
            }
        }

        public bool IsLive
        {
            get => isLive;
            set
            {
                if (CurrentHealth <= 0)
                {
                    isLive = false;
                    OnDeath?.Invoke();
                }
                else
                {
                    isLive = value;
                }
            }
        }

        protected virtual HealthInfo CreateHealthInfo()
        {
            return new HealthInfo();
        }

        protected virtual void ExecuteEventChangedHealth()
        {
            healthInfo.CurrentHealth = CurrentHealth;
            healthInfo.MaxHealth = MaxHealth;
            OnChangedHealth?.Invoke(healthInfo);
        }


        public virtual void Initialize()
        {
            healthInfo = CreateHealthInfo();
            MaxHealth = so_UnitHealth.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        protected virtual void OnDisable()
        {
            
        }

        
        public virtual void TakeDamage(IDamageable damageable)
        {
            var totalDamage = damageable.GetTotalDamage(gameObject);
            if(totalDamage == 0) return;
            
            Damaging = damageable.Owner;
            CurrentHealth -= totalDamage;
        }
    }
    
    public class HealthInfo
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
    }
}
