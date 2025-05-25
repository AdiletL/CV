using Calculate;

namespace Gameplay.Resistance
{
    public class PureDamageResistance : DamageResistance, IPureResistance
    {
        public UnitStatType UnitStatTypeID { get; } = UnitStatType.PureDamageResistance;
        public override DamageType DamageTypeID { get; } = DamageType.Pure;

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