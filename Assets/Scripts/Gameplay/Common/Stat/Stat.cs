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

        public void AddValue(float value)
        {
            CurrentValue += value;
            OnChangedCurrentValue?.Invoke();
        }

        public void RemoveValue(float value)
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
