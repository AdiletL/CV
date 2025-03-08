using Gameplay;
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
    public float Value { get; set; }
    public int GetTotalDamage(GameObject gameObject);
}
