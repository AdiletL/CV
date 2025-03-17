using System;
using Gameplay.Spawner;
using ScriptableObjects.Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit
{
    public abstract class UnitHealth : MonoBehaviour, IHealth, IActivatable
    {
        public event Action OnZeroHealth;
        
        [Inject] private DamagePopUpPopUpSpawner damagePopUpSpawner;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitHealth so_UnitHealth;

        protected UnitCenter unitCenter;
        protected float regenerationRate;
        
        public Stat HealthStat { get; } = new Stat();
        public Stat RegenerationStat { get; } = new Stat();
        
        public GameObject Damaging { get; protected set; }
        public bool IsLive { get; protected set; }
        public bool IsActive { get; protected set; }
        
        
        public virtual void Initialize()
        {
            HealthStat.AddMaxValue(so_UnitHealth.MaxHealth);
            HealthStat.AddCurrentValue(so_UnitHealth.MaxHealth);
            RegenerationStat.AddCurrentValue(so_UnitHealth.RegenerationHealthRate);
            unitCenter = gameObject.GetComponent<UnitCenter>();

            IsLive = HealthStat.CurrentValue > 0;
            ConvertingRegenerateRate();
            SubscribeEvent();
        }

        protected virtual void SubscribeEvent()
        {
            HealthStat.OnChangedCurrentValue += OnChangedHealthStatCurrentValue;
            RegenerationStat.OnChangedCurrentValue += OnChangedRegenerationStatCurrentValue;
        }
        protected virtual void UnsubscribeEvent()
        {
            RegenerationStat.OnChangedCurrentValue -= OnChangedRegenerationStatCurrentValue;
            HealthStat.OnChangedCurrentValue -= OnChangedHealthStatCurrentValue;
        }

        protected virtual void OnChangedHealthStatCurrentValue() => IsLive = HealthStat.CurrentValue > 0;
        protected virtual void OnChangedRegenerationStatCurrentValue() => ConvertingRegenerateRate();

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        
        
        protected void ConvertingRegenerateRate()
        {
            regenerationRate = Calculate.Convert.RegenerationToRate(RegenerationStat.CurrentValue);
        }
        
        protected virtual void Update()
        {
            if(!IsActive) return;
            RegenerationHealth();
        }
        
        private void RegenerationHealth()
        {
            if (HealthStat.CurrentValue <= HealthStat.MaximumValue)
                HealthStat.AddCurrentValue(Mathf.Min(regenerationRate * Time.deltaTime, HealthStat.MaximumValue));
        }

        public virtual void TakeDamage(DamageData damageData)
        {
            if(damageData.Amount < 0) 
                damageData.Amount = 0;
            
            Damaging = damageData.Owner;
            damagePopUpSpawner.CreatePopUp(unitCenter.Center.position, damageData.Amount);

            if (HealthStat.CurrentValue < damageData.Amount)
                HealthStat.RemoveCurrentValue(HealthStat.CurrentValue);
            else
                HealthStat.RemoveCurrentValue(damageData.Amount);
            
            if (HealthStat.CurrentValue <= 0)
                OnZeroHealth?.Invoke();
        }

        private void OnDestroy()
        {
            UnsubscribeEvent();
        }
    }
}
