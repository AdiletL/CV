
public enum UnitType
{
    nothing,
    player,
    creep,
}

public interface IUnit
{
    public UnitType UnitType { get; }
    
    public void Initialize();
}
