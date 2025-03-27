using Gameplay;
using UnityEngine;

public interface IMovement
{
    public Stat MovementSpeedStat { get; }
    public bool IsCanMove { get; }
    
    public void Initialize();
    public void ExecuteMovement();

    public void ActivateMovement();
    public void DeactivateMovement();
}