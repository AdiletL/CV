using Calculate;
using Gameplay;

public interface ICriticalDamage
{
    public Stat CriticalDamageStat { get; }
    public float GetCalculateDamage(float baseDamage);
    public bool TryApply();
}

public interface ICriticalDamageApplier
{
    public ICriticalDamage CriticalDamage { get; }
}
