using System;
using ScriptableObjects.Unit;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitEndurance : MonoBehaviour, IEndurance, IActivatable
    {
        public event Action<float, float> OnChangedEndurance;
        
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitEndurance so_UnitEndurance;
        
        public Stat EnduranceStat { get; } = new Stat();
        public Stat RegenerationStat { get; } = new Stat();
        public bool IsActive { get; protected set; }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        
        protected virtual void OnEnable()
        {
            EnduranceStat.OnAddCurrentValue += OnAddHealthStatCurrentValue;
            EnduranceStat.OnRemoveCurrentValue += OnRemoveHealthStatCurrentValue;
        }

        protected virtual void OnDisable()
        {
            EnduranceStat.OnAddCurrentValue -= OnAddHealthStatCurrentValue;
            EnduranceStat.OnRemoveCurrentValue -= OnRemoveHealthStatCurrentValue;
        }

        public virtual void Initialize()
        {
            EnduranceStat.AddMaxValue(so_UnitEndurance.MaxEndurance);
            EnduranceStat.AddValue(so_UnitEndurance.MaxEndurance);
        }
        
        
        protected virtual void OnAddHealthStatCurrentValue(float value)
        {
            OnChangedEndurance?.Invoke(EnduranceStat.CurrentValue, EnduranceStat.MaximumValue);
        }

        protected virtual void OnRemoveHealthStatCurrentValue(float value)
        {
            OnChangedEndurance?.Invoke(EnduranceStat.CurrentValue, EnduranceStat.MaximumValue);
        }
    }
}