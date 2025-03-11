using Gameplay;

public interface IHealth
{
    public Stat HealthStat { get; }
    public Stat RegenerationStat { get; }
    public bool IsLive { get; }
    public void Initialize();
}
