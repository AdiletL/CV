using Calculate;

namespace Gameplay.Resistance
{
    public class MagicalDamageResistance : DamageResistance, IMagicalResistance
    {
        public UnitStatType UnitStatTypeID { get; } = UnitStatType.MagicalDamageResistance;
        public override DamageType DamageTypeID { get; } = DamageType.Magical;

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