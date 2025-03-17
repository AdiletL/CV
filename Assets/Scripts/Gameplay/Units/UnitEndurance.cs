using System;
using ScriptableObjects.Unit;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitEndurance : MonoBehaviour, IEndurance, IActivatable
    {
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitEndurance so_UnitEndurance;
        
        protected float currentRegenerationRate;
        
        public Stat EnduranceStat { get; } = new Stat();
        public Stat RegenerationStat { get; } = new Stat();
        public bool IsActive { get; protected set; }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        

        public virtual void Initialize()
        {
            EnduranceStat.AddMaxValue(so_UnitEndurance.MaxEndurance);
            EnduranceStat.AddCurrentValue(so_UnitEndurance.MaxEndurance);
            RegenerationStat.AddCurrentValue(so_UnitEndurance.RegenerationEnduranceRate);
            
            ConvertingRegenerateRate();
            SubscribeEvent();
        }
        
        protected virtual void SubscribeEvent()
        {
            RegenerationStat.OnChangedCurrentValue += OnChangedRegenerationStatCurrentValue;
        }
        protected virtual void UnsubscribeEvent()
        {
            RegenerationStat.OnChangedCurrentValue -= OnChangedRegenerationStatCurrentValue;
        }
        
        protected virtual void OnChangedRegenerationStatCurrentValue() => ConvertingRegenerateRate();
        
        protected void ConvertingRegenerateRate()
        {
            currentRegenerationRate = Calculate.Convert.RegenerationToRate(RegenerationStat.CurrentValue);
        }
        
        protected virtual void Update()
        {
            if(!IsActive) return;
            RegenerationEndurance();
        }
        
        private void RegenerationEndurance()
        {
            if (EnduranceStat.CurrentValue <= EnduranceStat.MaximumValue)
                EnduranceStat.AddCurrentValue(Mathf.Min(currentRegenerationRate * Time.deltaTime, EnduranceStat.MaximumValue));
        }

        private void OnDestroy()
        {
            UnsubscribeEvent(); 
        }
    }
}