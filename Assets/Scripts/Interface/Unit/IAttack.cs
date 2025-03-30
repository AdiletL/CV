using Gameplay;

public interface IAttack : IApplyDamage
{
    public Stat DamageStat { get; }
    public Stat AttackSpeedStat { get; }
    public Stat RangeAttackStat { get; }

    public void Initialize();

    public void Attack();
}

public interface IApplyDamage
{
    public DamageData DamageData { get; }
    public void ApplyDamage();
}
