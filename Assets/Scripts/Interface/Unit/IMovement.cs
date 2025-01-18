using UnityEngine;

public interface IMovement
{
    public void Initialize();
    public float MovementSpeed { get; }
    public void ExecuteMovement();
}