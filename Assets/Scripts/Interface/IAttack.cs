
using Unit;

public interface IAttack : IApplyDamage
{
    public int AmountAttack { get; }
    public void Initialize();

    public void Attack();
    
    public void IncreaseStates(IState state);
}

public interface IApplyDamage
{
    IDamageable Damageable { get; set; }
    public void ApplyDamage();
}
