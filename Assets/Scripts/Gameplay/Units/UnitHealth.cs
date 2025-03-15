using System;
using Gameplay.Spawner;
using ScriptableObjects.Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit
{
    public abstract class UnitHealth : MonoBehaviour, IHealth, IActivatable
    {
        public event Action<float, float> OnChangedHealth;
        public event Action OnZeroHealth;
        
        [Inject] private DamagePopUpPopUpSpawner damagePopUpSpawner;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitHealth so_UnitHealth;

        protected UnitCenter unitCenter;
        
        public Stat HealthStat { get; } = new Stat();
        public Stat RegenerationStat { get; } = new Stat();
        
        public GameObject Damaging { get; protected set; }
        public bool IsLive { get; protected set; }
        public bool IsActive { get; protected set; }
        
        
        protected virtual void OnEnable()
        {
            HealthStat.OnChangedCurrentValue += OnChangedHealthStatCurrentValue;
        }

        protected virtual void OnDisable()
        {
            HealthStat.OnChangedCurrentValue -= OnChangedHealthStatCurrentValue;
        }

        public virtual void Initialize()
        {
            HealthStat.AddMaxValue(so_UnitHealth.MaxHealth);
            HealthStat.AddValue(so_UnitHealth.MaxHealth);
            unitCenter = gameObject.GetComponent<UnitCenter>();
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        
        protected virtual void OnChangedHealthStatCurrentValue()
        {
            IsLive = HealthStat.CurrentValue > 0;
            OnChangedHealth?.Invoke(HealthStat.CurrentValue, HealthStat.MaximumValue);
        }

        public virtual void TakeDamage(DamageData damageData)
        {
            if(damageData.Amount < 0) 
                damageData.Amount = 0;
            
            Damaging = damageData.Owner;
            HealthStat.RemoveValue(damageData.Amount);
            damagePopUpSpawner.CreatePopUp(unitCenter.Center.position, damageData.Amount);

            if (HealthStat.CurrentValue <= 0)
                OnZeroHealth?.Invoke();
        }
    }
}
