using UnityEngine;

public interface IProjectile : IMovement, IApplyDamage
{
    public AnimationCurve moveCurve { get; }

}
