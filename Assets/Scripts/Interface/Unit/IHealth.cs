
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
    public int CurrentHealth { get; }
    public int MaxHealth { get; }
}

public interface IDamageable
{
    public GameObject Owner { get; }
    public int Amount { get; }
    public int AdditionalDamage { get; }
    public void AddAdditionalDamage(int value);
    public void RemoveAdditionalDamage(int value);
    
    public int GetTotalDamage(GameObject gameObject);
}
