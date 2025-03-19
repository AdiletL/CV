using UnityEngine;

public interface ITrap
{
    public LayerMask EnemyLayer { get; } 
    
    public void Initialize();

    public void StartAction();
    public void ResetAction();
}

public interface ITrapInteractable : IInteractable
{
    
}

public interface ITrapAttackable : IAttackable, ITrapInteractable
{
    
}
