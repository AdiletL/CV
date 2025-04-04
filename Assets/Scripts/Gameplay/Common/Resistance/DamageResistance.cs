using Calculate;

namespace Gameplay.Resistance
{
    public abstract class DamageResistance : IDamageResistance
    {
        public ResistanceType ResistanceTypeID { get; } = ResistanceType.Damage;
        public Stat ResistanceStat { get; } = new Stat();
        public ValueType ValueType { get; protected set; }
        public abstract DamageType DamageTypeID { get; }
        
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