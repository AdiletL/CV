using Gameplay;

public interface IMagicalResistance : IResistance
{
    public StatType StatTypeID { get; }
    public DamageType DamageTypeID { get; }
}
