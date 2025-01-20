
using Unit;

public interface IAttack : IApplyDamage
{
    public int AttackSpeed { get; }
    public void Initialize();

    public void Attack();
    
    public void IncreaseAttackSpeed(int amount);
    public void DecreaseAttackSpeed(int amount);
}

public interface IApplyDamage
{
    public IDamageable Damageable { get; }
    public void ApplyDamage();
}
