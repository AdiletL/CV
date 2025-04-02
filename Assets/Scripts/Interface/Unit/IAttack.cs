using System.Collections.Generic;
using Gameplay;

public interface IAttack : IApplyDamage, IDamage, IRangeAttack, IAttackSpeed
{
    public void Initialize();

    public void Attack();
}

public interface IApplyDamage
{
    public DamageData DamageData { get; }
    public void ApplyDamage();
}
