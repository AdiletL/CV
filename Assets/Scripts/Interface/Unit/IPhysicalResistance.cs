using Gameplay;

public interface IPhysicalResistance : IResistance
{
   public StatType StatTypeID { get; }
   public DamageType DamageTypeID { get; }
}
