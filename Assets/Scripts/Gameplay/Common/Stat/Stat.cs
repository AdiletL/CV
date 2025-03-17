using System;
using UnityEngine.Serialization;
using ValueType = Calculate.ValueType;

namespace Gameplay
{
    public enum StatValueType
    {
        Nothing,
        Current,
        Minimum,
        Maximum,
    }
    public enum StatType
    {
        Nothing,
        Damage,
        AttackSpeed,
        AttackRange,
        MovementSpeed,
        Health,
        RegenerationHealth,
        Mana,
        RegenerationMana,
        Endurance,
        RegenerationEndurance,
        Armor,
        MagicalResistance,
        PureResistance,
        Level,
        Experience,
    }

    [System.Serializable]
    public class StatConfig
    {
        public StatType StatTypeID;
        public StatValueConfig[] StatValuesConfig;
    }

    [System.Serializable]
    public class StatValueConfig
    {
        public StatValueType StatValueTypeID;
        public ValueType ValueTypeID;
        public float Value;
    }
    
    [System.Serializable]
    public class StatConfigData
    {
        public StatConfig[] StatConfigs;
    }
    
    public class Stat
    {
        public event Action OnChangedCurrentValue;
        public event Action OnChangedMinimumValue;
        public event Action OnChangedMaximumValue;
        
        public float CurrentValue { get; private set; }
        public float MinimumValue { get; private set; }
        public float MaximumValue { get; private set; }

        public float GetValue(StatValueType statValueType)
        {
            switch (statValueType)
            {
                case StatValueType.Current: return CurrentValue;
                case StatValueType.Minimum: return MinimumValue;
                case StatValueType.Maximum: return MaximumValue;
                default: throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null);
            }
        }
        
        public void AddValue(float value, StatValueType statValueType)
        {
            switch (statValueType)
            {
                case StatValueType.Current: AddCurrentValue(value); break;
                case StatValueType.Minimum: AddMinValue(value); break;
                case StatValueType.Maximum: AddMaxValue(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null);
            }
        }

        public void RemoveValue(float value, StatValueType statValueType)
        {
            switch (statValueType)
            {
                case StatValueType.Current: RemoveCurrentValue(value); break;
                case StatValueType.Minimum: RemoveMinValue(value); break;
                case StatValueType.Maximum: RemoveMaxValue(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null);
            }
        }
        
        public void AddCurrentValue(float value)
        {
            CurrentValue += value;
            OnChangedCurrentValue?.Invoke();
        }

        public void RemoveCurrentValue(float value)
        {
            CurrentValue -= value;
            OnChangedCurrentValue?.Invoke();
        }
        
        public void AddMinValue(float value)
        {
            MinimumValue += value;
            OnChangedMinimumValue?.Invoke();
        }

        public void RemoveMinValue(float value)
        {
            MinimumValue -= value;
            OnChangedMinimumValue?.Invoke();
        }

        public void AddMaxValue(float value)
        {
            MaximumValue += value;
            OnChangedMaximumValue?.Invoke();
        }

        public void RemoveMaxValue(float value)
        {
            MaximumValue -= value;
            OnChangedMaximumValue?.Invoke();
        }
        
    }
}
