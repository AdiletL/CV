
using System.Threading.Tasks;
using UnityEngine;

public interface IWeapon : IApplyDamage
{
    GameObject WeaponPrefab { get; }
    GameObject CurrentTarget { get; }
    public int AmountAttack { get; }
    
    public void Initialize();

    public Task FireAsync();
    public void SetTarget(GameObject target);
    public void IncreaseStates(Unit.IState state);
}
