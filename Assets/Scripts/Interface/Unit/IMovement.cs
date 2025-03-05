using Unit;
using UnityEngine;

public interface IMovement
{
    public Stat MovementSpeedStat { get; }
    public void Initialize();
    public void ExecuteMovement();
}