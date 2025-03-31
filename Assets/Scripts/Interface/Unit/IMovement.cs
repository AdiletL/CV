using Gameplay;
using UnityEngine;

public interface IMovement : IMovementSpeed
{
    public bool IsCanMove { get; }
    
    public void Initialize();
    public void ExecuteMovement();

    public void ActivateMovement();
    public void DeactivateMovement();
}