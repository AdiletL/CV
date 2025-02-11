using UnityEngine;

public interface IMovement
{
    public float MovementSpeed { get; }
    public void Initialize();
    public void ExecuteMovement();
}