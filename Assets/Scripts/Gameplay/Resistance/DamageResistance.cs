using Calculate;

namespace Gameplay.Resistance
{
    public class DamageResistance : IResistance
    {
        public StatType StatTypeID { get; }
        public DamageType DamageType { get; }
        public ValueType ValueType { get; }
        public Stat ProtectionStat { get; } = new();

        public DamageResistance(StatType statTypeID, DamageType damageType, ValueType valueType, float value)
        {
            StatTypeID = statTypeID;
            DamageType = damageType;
            ValueType = valueType;
            ProtectionStat.AddValue(value);
        }

        public DamageData DamageModify(DamageData damageData)
        {
            if (damageData.DamageTypeID.HasFlag(DamageType))
            {
                var gameValue = new GameValue(ProtectionStat.CurrentValue * 2, ValueType);
                damageData.Amount -= gameValue.Calculate(damageData.Amount);
            }
            return damageData;
        }
    }
}