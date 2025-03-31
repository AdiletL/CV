using Calculate;

namespace Gameplay.Resistance
{
    public class PhysicalDamageResistance : IPhysicalResistance
    {
        public StatType StatTypeID { get; } = StatType.PhysicalDamageResistance;
        public DamageType DamageTypeID { get; } = DamageType.Physical;
        public ValueType ValueType { get; }
        public Stat ResistanceStat { get; } = new();

        public PhysicalDamageResistance(ValueType valueType, float value)
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