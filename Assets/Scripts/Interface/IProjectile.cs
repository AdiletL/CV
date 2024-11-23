using UnityEngine;

public interface IProjectile : IMove, IApplyDamage
{
    public AnimationCurve moveCurve { get; }

    public void Initialize();
}
