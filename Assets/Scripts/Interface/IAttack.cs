
using Unit;

public interface IAttack : IApplyDamage
{
    public float AmountAttack { get; set; }
    public void Initialize();

    public void IncreaseStates(IState state);
}

public interface IApplyDamage
{
    public IDamageble Damageble { get; set; }
    public void ApplyDamage();
}
