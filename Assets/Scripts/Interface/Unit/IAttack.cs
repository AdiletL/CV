
using Unit;

public interface IAttack : IApplyDamage
{
    public float AmountAttack { get; }
    public void Initialize();

    public void Attack();
    
    public void IncreaseStates(IState state);
}

public interface IApplyDamage
{
    public IDamageable Damageable { get; }
    public void ApplyDamage();
}
