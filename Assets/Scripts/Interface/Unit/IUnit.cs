
public enum UnitType
{
    nothing,
    player,
    creep,
    trap,
    reward,
    platform,
    environment
}

public interface IUnit
{
    public UnitType UnitType { get; }
    
    public void Initialize();

    public void Show();
    public void Hide();
}
