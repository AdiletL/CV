using Gameplay;
using UnityEngine;

public interface IWeapon : IApplyDamage
{
    public Stat DamageStat { get; }
    
    public void Initialize();

    public void SetInParent(Transform parent);
}
