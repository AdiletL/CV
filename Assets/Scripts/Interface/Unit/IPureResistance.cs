using Gameplay;

public interface IPureResistance : IResistance
{
    public StatType StatTypeID { get; }
    public DamageType DamageTypeID { get; }
}
