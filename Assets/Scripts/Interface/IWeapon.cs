using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IWeapon : IApplyDamage
{
    GameObject WeaponPrefab { get; }
    GameObject CurrentTarget { get; }
    
    public void Initialize();

    public UniTask FireAsync();
    public void SetTarget(GameObject target);
    public void IncreaseStates(Unit.IState state);
}
