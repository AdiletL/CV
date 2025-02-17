using UnityEngine;

public interface IMovement
{
    public float BaseMovementSpeed { get; }
    public float CurrentMovementSpeed { get; }
    public void Initialize();
    public void ExecuteMovement();
}