using UnityEngine;

public interface IMove
{
    public float MovementSpeed { get; }
    public void Move();
}