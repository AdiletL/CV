using Calculate;

namespace Gameplay.Resistance
{
    public class NormalDamageResistance : IResistance
    {
        public int Value { get; }
        public ValueType ValueType { get; }

        public NormalDamageResistance(int value, ValueType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }
    }

    [System.Serializable]
    public class NormalDamageResistanceInfo
    {
        public int Value { get; }
        public ValueType ValueType { get; }
    }
}