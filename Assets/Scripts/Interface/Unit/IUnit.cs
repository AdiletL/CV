
public enum UnitType
{
    nothing,
    player,
    creep,
    trap,
    reward
}

public interface IUnit
{
    public UnitType UnitType { get; }
    
    public void Initialize();
}
