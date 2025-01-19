using UnityEngine;

public interface IHealth
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; }
    public bool IsLive { get; }
    public void Initialize();
}

public interface IDamageable
{
    public GameObject Owner { get; }
    public int CurrentDamage { get; }
    public int AdditionalDamage { get; }
    public void AddAdditionalDamage(int value);
    public void RemoveAdditionalDamage(int value);
    
    public int GetTotalDamage(GameObject gameObject);
}
