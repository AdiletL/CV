using Calculate;

namespace Gameplay.Resistance
{
    public class PhysicalDamageResistance : DamageResistance, IPhysicalResistance
    {
        public UnitStatType UnitStatTypeID { get; } = UnitStatType.PhysicalDamageResistance;
        public override DamageType DamageTypeID { get; } = DamageType.Physical;

        public PhysicalDamageResistance(ValueType valueType, float value)
        {
            ValueType = valueType;
            ResistanceStat.AddCurrentValue(value);
        }
    }
}