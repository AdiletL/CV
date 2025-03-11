using UnityEngine;

public interface ITrap
{
    public GameObject CurrentTarget { get; }
    public LayerMask EnemyLayer { get; } 
    
    public void Initialize();
    
    public void Trigger();
    public void Reset();
}

public interface ITrapInteractable : IInteractable
{
    
}

public interface ITrapAttackable : IAttackable, ITrapInteractable
{
    
}
