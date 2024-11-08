
using System;
using Unit;
using UnityEngine;

public interface IHealth
{
    public event Action<IHealthInfo> OnChangedHealth;
    public int MaxHealth { get; set; }
    public int CurrentHealth { get;  set; }
    public bool IsLive {get;set;}
    public void Initialize();
    public void IncreaseStates(IState state);
    public void TakeDamage(IDamageble damageble);
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
