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
        PhysicalResistance,
        MagicalResistance,
        PureResistance,
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
        public event Action<float> OnAddCurrentValue;
        public event Action<float> OnRemoveCurrentValue;
        public event Action<float> OnAddMinimumValue;
        public event Action<float> OnRemoveMinimumValue;
        public event Action<float> OnAddMaximumValue;
        public event Action<float> OnRemoveMaximumValue;
        
        public float CurrentValue { get; private set; }
        public float MinimumValue { get; private set; }
        public float MaximumValue { get; private set; }

        public void AddValue(float value)
        {
            CurrentValue += value;
            OnAddCurrentValue?.Invoke(value);
        }

        public void RemoveValue(float value)
        {
            CurrentValue -= value;
            OnRemoveCurrentValue?.Invoke(value);
        }
        
        public void AddMinValue(float value)
        {
            MinimumValue += value;
            OnAddMinimumValue?.Invoke(value);
        }

        public void RemoveMinValue(float value)
        {
            MinimumValue -= value;
            OnRemoveMinimumValue?.Invoke(value);
        }

        public void AddMaxValue(float value)
        {
            MaximumValue += value;
            OnAddMaximumValue?.Invoke(value);
        }

        public void RemoveMaxValue(float value)
        {
            MaximumValue -= value;
            OnRemoveMaximumValue?.Invoke(value);
        }
    }
}
