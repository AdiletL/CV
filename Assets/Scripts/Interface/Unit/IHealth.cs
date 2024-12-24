
using System;
using Unit;
using UnityEngine;

public interface IHealth
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; }
    public bool IsLive { get; }
    public void Initialize();
    public void IncreaseStates(IState state);
    public void TakeDamage(IDamageable damageable);
}

public interface IHealthInfo
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}

public interface IDamageable
{
    public GameObject Owner { get; set; }
    public int Amount { get; }
    public int GetTotalDamage(GameObject gameObject);
}
