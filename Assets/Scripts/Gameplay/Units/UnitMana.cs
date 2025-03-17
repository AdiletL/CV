using ScriptableObjects.Unit;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitMana : MonoBehaviour, IMana, IActivatable
    {
        [SerializeField] protected UnitController unitController;
        [SerializeField] protected SO_UnitMana so_UnitMana;
        
        protected float regenerationRate;
        
        public Stat ManaStat { get; } = new Stat();
        public Stat RegenerationStat { get; } = new Stat();
        
        public bool IsActive { get; protected set; }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        public virtual void Initialize()
        {
            ManaStat.AddMaxValue(so_UnitMana.MaxMana);
            ManaStat.AddCurrentValue(so_UnitMana.MaxMana); 
            RegenerationStat.AddCurrentValue(so_UnitMana.RegenerationManaRate);
            
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
            regenerationRate = Calculate.Convert.RegenerationToRate(RegenerationStat.CurrentValue);
        }
        
        protected virtual void Update()
        {
            if(!IsActive) return;
            RegenerationMana();
        }
        
        private void RegenerationMana()
        {
            if (ManaStat.CurrentValue <= ManaStat.MaximumValue)
                ManaStat.AddCurrentValue(Mathf.Min(regenerationRate * Time.deltaTime, ManaStat.MaximumValue));
        }

        private void OnDestroy()
        {
            UnsubscribeEvent();
        }
    } 
}