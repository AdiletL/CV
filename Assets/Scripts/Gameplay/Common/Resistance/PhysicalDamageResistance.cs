using Calculate;

namespace Gameplay.Resistance
{
    public class PhysicalDamageResistance : DamageResistance, IPhysicalResistance
    {
        public StatType StatTypeID { get; } = StatType.PhysicalDamageResistance;
        public override DamageType DamageTypeID { get; } = DamageType.Physical;

        public PhysicalDamageResistance(ValueType valueType, float value)
        {
            ValueType = valueType;
            ResistanceStat.AddCurrentValue(value);
        }
    }
}