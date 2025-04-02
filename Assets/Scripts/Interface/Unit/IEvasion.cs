using Gameplay;

public interface IEvasion
{
    public Stat EvasionStat { get; }
    public bool TryEvade();
}

public interface IEvasionApplier
{
    public IEvasion Evasion { get; }
}