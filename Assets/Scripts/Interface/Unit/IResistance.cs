using Calculate;
using Gameplay;

public enum ResistanceType
{
    Nothing,
    Damage,
    Movement,
}

public interface IResistance
{
    public ResistanceType ResistanceTypeID { get; }
    public Stat ResistanceStat { get; }
    public ValueType ValueType { get; }
}

public interface IDamageResistance : IResistance
{
    public DamageType DamageTypeID { get; }
    public DamageData DamageModify(DamageData damageData);
}

public interface IPhysicalResistance : IDamageResistance
{
    public UnitStatType UnitStatTypeID { get; }
}

public interface IMagicalResistance : IDamageResistance
{
    public UnitStatType UnitStatTypeID { get; }
}

public interface IPureResistance : IDamageResistance
{
    public UnitStatType UnitStatTypeID { get; }
}
