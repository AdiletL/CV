
using UnityEngine;

public interface IHealth
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsLive {get;set;}
    public void Initialize();
    public void TakeDamage(IDamageble damageble);
}

public interface IDamageble
{
    public int amount { get; set; }
    public int GetTotalDamage(GameObject gameObject);
}
