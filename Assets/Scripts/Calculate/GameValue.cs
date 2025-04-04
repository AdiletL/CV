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

        public float Calculate(float baseValue)
        {
            float result = valueType == ValueType.Percent ? baseValue * value : value;
            return (float)(Math.Truncate(result * 10) / 10);
        }
    }

    [System.Serializable]
    public class GameValueConfig
    {
        public ValueType ValueTypeID;
        public float Value;
    }
}