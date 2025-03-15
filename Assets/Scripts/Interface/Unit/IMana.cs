using Gameplay;

public interface IMana
{
    public Stat ManaStat { get; }
    public Stat RegenerationStat { get; }
    
    public void Initialize();
}
