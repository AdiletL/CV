
public enum UnitType
{
    nothing,
    player,
    creep,
    trap,
    tower,
}

public interface IUnit
{
    public UnitType UnitType { get; }
    
    public void Initialize();
}
