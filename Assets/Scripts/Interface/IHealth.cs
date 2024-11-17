
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
    public void TakeDamage(IDamageble damageble, GameObject gameObject);
}

public interface IHealthInfo
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}

public interface IDamageble
{
    public int Amount { get; set; }
    public int GetTotalDamage(GameObject gameObject);
}
