using UnityEngine;

public interface IProjectile : IMove
{
    public AnimationCurve moveCurve { get; }
}
