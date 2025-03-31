using Calculate;

namespace Gameplay.Resistance
{
    public class MagicalDamageResistance : IMagicalResistance
    {
        public StatType StatTypeID { get; } = StatType.MagicalDamageResistance;
        public DamageType DamageTypeID { get; } = DamageType.Magical;
        public ValueType ValueType { get; }
        public Stat ResistanceStat { get; } = new();

        public MagicalDamageResistance(ValueType valueType, float value)
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