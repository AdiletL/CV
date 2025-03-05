using Unit;
using UnityEngine;

public interface IHealth
{
    public Stat HealthStat { get; }
    public Stat RegenerationStat { get; }
    public bool IsLive { get; }
    public void Initialize();
}

public interface IDamageable
{
    public GameObject Owner { get; }
    public Stat DamageStat { get; }
    public int GetTotalDamage(GameObject gameObject);
}
