using UnityEngine;

public interface IMove
{
    public float MovementSpeed { get; set; }
    public void Move();
}