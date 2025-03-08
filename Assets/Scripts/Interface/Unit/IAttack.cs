
using Unit;

public interface IAttack : IApplyDamage
{
    public Stat AttackSpeedStat { get; }
    public Stat DamageStat { get; }

    public void Initialize();

    public void Attack();
}

public interface IApplyDamage
{
    public IDamageable Damageable { get; }
    public void ApplyDamage();
}
