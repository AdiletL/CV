using Gameplay;

public interface IEndurance
{
    public Stat EnduranceStat { get; }
    public Stat RegenerationStat { get; }

    public void Initialize();
}
