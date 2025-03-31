using Calculate;

namespace Gameplay.Resistance
{
    public class DamageResistance : IResistance
    {
        public StatType StatTypeID { get; }
        public DamageType DamageType { get; private set; }
        public ValueType ValueType { get; }
        public Stat ProtectionStat { get; } = new();

        public DamageResistance(StatType statType, ValueType valueType, float value)
        {
            this.StatTypeID = statType;
            ValueType = valueType;
            ProtectionStat.AddCurrentValue(value);
            switch (statType)
            {
                case StatType.Armor: DamageType = DamageType.Physical; break;
                case StatType.MagicalResistance: DamageType = DamageType.Magical; break;
                case StatType.PureResistance: DamageType = DamageType.Pure; break;
            }
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