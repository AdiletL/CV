using UnityEngine;

public interface IPatrol : IMovement
{
    public Vector3[] PatrolPoints { get; }
}
