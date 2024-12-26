using System;

namespace Calculate
{
    public enum ValueType
    {
        Fixed, 
        Percent 
    }

    public struct GameValue
    {
        private float value;         
        private ValueType valueType;    

        public GameValue(float value, ValueType valueType)
        {
            this.value = value;
            this.valueType = valueType;
        }

        public int Calculate(float baseValue)
        {
            float result = valueType == ValueType.Percent ? (baseValue * value) / 100f : value;
            return (int)(Math.Truncate(result * 10) / 10);
        }
    }
}