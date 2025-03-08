using Gameplay.Equipment.Weapon;
using Unit;
using UnityEngine;

public interface IProjectile : IMovement, IApplyDamage
{
    public AnimationCurve MoveCurve { get; }
}
