using System;
using ScriptableObjects.Unit;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitHealth : MonoBehaviour, IHealth, IActivatable
    {
        public event Action<float, float> OnChangedHealth;
        public event Action OnZeroHealth;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitHealth so_UnitHealth;
        
        
        public GameObject Damaging { get; private set; }
        public Stat HealthStat { get; } = new Stat();
        public Stat RegenerationStat { get; } = new Stat();
        public bool IsLive { get; protected set; }
        public bool IsActive { get; protected set; }
        
        
        protected virtual void OnEnable()
        {
            HealthStat.OnAddCurrentValue += OnAddHealthStatCurrentValue;
            HealthStat.OnRemoveCurrentValue += OnRemoveHealthStatCurrentValue;
        }

        protected virtual void OnDisable()
        {
            HealthStat.OnAddCurrentValue -= OnAddHealthStatCurrentValue;
            HealthStat.OnRemoveCurrentValue -= OnRemoveHealthStatCurrentValue;
        }

        public virtual void Initialize()
        {
            HealthStat.AddMaxValue(so_UnitHealth.MaxHealth);
            HealthStat.AddValue(so_UnitHealth.MaxHealth);
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        
        protected virtual void OnAddHealthStatCurrentValue(float value)
        {
            IsLive = HealthStat.CurrentValue > 0;
            OnChangedHealth?.Invoke(HealthStat.CurrentValue, HealthStat.MaximumValue);
        }

        protected virtual void OnRemoveHealthStatCurrentValue(float value)
        {
            IsLive = HealthStat.CurrentValue > 0;
            OnChangedHealth?.Invoke(HealthStat.CurrentValue, HealthStat.MaximumValue);
        }

        public virtual void TakeDamage(IDamageable damageable)
        {
            var totalDamage = damageable.GetTotalDamage(gameObject);
            Damaging = damageable.Owner;
            HealthStat.RemoveValue(totalDamage);

            if (HealthStat.CurrentValue <= 0)
                OnZeroHealth?.Invoke();
        }
    }
}
