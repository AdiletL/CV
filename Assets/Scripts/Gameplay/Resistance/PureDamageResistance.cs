using Calculate;

namespace Gameplay.Resistance
{
    public class PureDamageResistance : IPureResistance
    {
        public StatType StatTypeID { get; } = StatType.PureDamageResistance;
        public DamageType DamageTypeID { get; } = DamageType.Pure;
        public ValueType ValueType { get; }
        public Stat ResistanceStat { get; } = new();

        public PureDamageResistance(ValueType valueType, float value)
        {
            ValueType = valueType;
            ResistanceStat.AddCurrentValue(value);
        }

        public DamageData DamageModify(DamageData damageData)
        {
            if (damageData.DamageTypeID.HasFlag(DamageTypeID))
            {
                var gameValue = new GameValue(ResistanceStat.CurrentValue * 2, ValueType);
                damageData.Amount -= gameValue.Calculate(damageData.Amount);
            }
            return damageData;
        }
    }
}